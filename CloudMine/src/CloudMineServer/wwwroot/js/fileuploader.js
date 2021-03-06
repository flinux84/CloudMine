﻿var TheFileUploader = function (progressbar) {

    var progress = progressbar;
    jQuery.sha1 = sha1;
    var MaxFileSizeMB = 1;
    var FileStreamPos = 0;
    var TotalCount;
    var FileItem = {};
    var ChunkArray = [];
    var FileID;
    var actualFile;
    var ind;

    TheFileUploader.prototype.Upload = function (file, index) {
        actualFile = file;
        ind = index;
        ShowLoading();
        GetSHA1();
        return FileID;
    }

    //Läser checksum för filen som sedan skickas iväg som metadata.
    function GetSHA1() {
        var reader = new FileReader();
        reader.onload = function (event) {
            var binary = event.target.result;
            var hashCode = $.sha1(binary);
            MakeFileItem(hashCode);
        };
        reader.readAsArrayBuffer(actualFile);
    }

    //Funktionen kallas på när checksum är OK, skapar ett objekt av fil-elementen.
    function MakeFileItem(hashCode) {
        FileItem.checksum = hashCode;
        FileItem.fileName = actualFile.name;
        FileItem.dataType = actualFile.name.split('.').pop();;
        FileItem.fileSize = actualFile.size;
        FileItem.description = "";
        theFileItem = JSON.stringify(FileItem);
        SendMetaData(theFileItem);
    };

    //Här skickar vi metadatan och får tillbaks data.
    function SendMetaData(theFileItem) {
        $.ajax({
            type: "POST",
            url: '../api/v1.0/FileItems/',
            contentType: 'application/json',
            dataType: 'json',
            data: theFileItem,
            error: function (e, jqHXR) {
                console.log(e);
                if (e.status == 409) {                    
                    alert("The file already exists");
                    HideLoading();
                    return;
                }
                if (e.status === 401) {
                    alert("Please login");
                    HideLoading();
                    return;
                }
                HideLoading();
            },
            //Är det ok, så påbörjar vi metoden med att skicka datachunks av filen.
            success: function (result, status, jqHXR) {
                Datatype: "json",
                console.log("File metadata sent");
                if(jqHXR.status == 202) {
                console.log("Partial file, resuming upload.");
                }
                UploadChunks(result);
            }
        })
    };

    //Laddar upp chunksen
    function UploadChunks(result) {
        ShowLoading();
        FileID = result.id;
        GetFileItem(FileID);
        var EndPos = MaxFileSizeMB * (1024 * 1024);
        var BufferChunkSize = MaxFileSizeMB * (1024 * 1024);
        var Size = actualFile.size;

        while (FileStreamPos < Size) {
            ChunkArray.push(actualFile.slice(FileStreamPos, EndPos));
            FileStreamPos = EndPos; // Hoppar för varje läst fil.
            EndPos = FileStreamPos + BufferChunkSize; // sätter nästa chunk-längd.
        }
        TotalCount = ChunkArray.length;
        var PartCount = 0;
        HideLoading();
        SendNextPart(ChunkArray, PartCount);
    };

    //skickar nästa chunk efter att ha genererat checksum för chunken
    function SendNextPart(ChunkArray, PartCount) {
        var chunk = ChunkArray.shift();
        if (chunk == null) {
            GetFileItem(FileID);            
            return;}
        PartCount++;

        blob = new Blob([chunk], { type: 'application/octet-binary' });

        var promise = new Promise(makeChunkChecksum);
        promise.then(function (data) {

            var FilePartName = actualFile.name + ".part_" + PartCount + "." + TotalCount;
            var byteData = data.byteArray;
            var FileChunk = {};

            FileChunk.CheckSum = data.hashCode2;
            FileChunk.PartName = FilePartName;
            FileChunk.Data = byteData;
            FileChunk.FileItemId = FileID;

            var FD = new FormData();
            FD.append('PartName', FilePartName);
            FD.append('Checksum', data.hashCode2);
            FD.append('Data', chunk);
            FD.append('FileItemId', FileID);

            $.ajax({
                type: "POST",
                url: '../api/v1.0/FileItems/' + FileID,
                contentType: false,
                processData: false,
                data: FD,
                error: function (e) {
                    console.log(e);
                    if (e.status == 409) {
                        var percent = Math.round((PartCount / TotalCount) * 100)
                        progress.updateProgress(percent, actualFile.name, ind);
                        console.log("Uploaded " + FilePartName)
                        SendNextPart(ChunkArray, PartCount);
                    }
                    else {
                        console.log("annat fel");
                    }                  
                },
                success: function (result) {
                    var jsonUpdateData = result;
                    Datatype: false;
                    var percent = Math.round((PartCount / TotalCount) * 100)
                    progress.updateProgress(percent, actualFile.name, ind);
                    console.log("Uploaded " + FilePartName)
                    SendNextPart(ChunkArray, PartCount)
                }
            });
        });

        //Läser checksum och binär data för chunken
        function makeChunkChecksum(resolve) {

            var reader = new FileReader();
            reader.onload = function (event) {
                var binary = event.target.result;
                var hashCode2 = $.sha1(binary);
                var theObject = {hashCode2};
                resolve(theObject);
            };
            reader.readAsArrayBuffer(blob);
        }
    }
}