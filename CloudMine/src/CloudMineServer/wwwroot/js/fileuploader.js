var TheFileUploader = function (progressbar) {

    var progress = progressbar;
    jQuery.sha1 = sha1;
    var MaxFileSizeMB = 1;
    var FileStreamPos = 0;
    var TotalCount;
    var FileItem = {};
    var ChunkArray = [];
    var FileId;
    var actualFile;

    TheFileUploader.prototype.Upload = function (file) {
        actualFile = file;
        GetSHA1();
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
            error: function (e) {
                console.log(e);
            },
            //Är det ok, så påbörjar vi metoden med att skicka datachunks av filen.
            success: function (result, status, jqHXR) {
                Datatype: "json",
                console.log("Första");
                UploadChunks(result);
            }
        });
    };

    //Laddar upp chunksen
    function UploadChunks(result) {
        FileID = result.id;
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
        SendNextPart(ChunkArray, PartCount);
    };

    //skickar nästa chunk efter att ha genererat checksum för chunken
    function SendNextPart(ChunkArray, PartCount) {
        var chunk = ChunkArray.shift();
        if (chunk == null) { return; }
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
                    if(e = 409){
                        SendNextPart(ChunkArray, PartCount);
                        var percent = Math.round((PartCount / TotalCount) * 100)
                        progress.updateProgress(percent, actualFile.name);
                    }
                },
                success: function (result) {
                    var jsonUpdateData = result;
                    Datatype: false;
                    SendNextPart(ChunkArray, PartCount)
                    var percent = Math.round((PartCount / TotalCount) * 100)
                    progress.updateProgress(percent, actualFile.name);
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