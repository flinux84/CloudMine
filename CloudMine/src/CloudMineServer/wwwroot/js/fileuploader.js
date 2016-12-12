$(document).ready(function () {

    jQuery.sha1 = sha1;
    var filename;
    var TargetFile;
    var Id;
    var Checksum;
    var FileName;
    var Description;
    var Uploaded;
    var Private = true;
    var Datatype;
    var Filesize;
    var Userid = null;
    var DataChunks;
    var ObjectInfo = [];
    var ObjectElement = {};
    var theInput;
    var theFile;
    var ChunkElement = {};
    var FileChunk = [];    
    var MaxFileSizeMB = 1;
    var BufferChunkSize = MaxFileSizeMB * (1024 * 1024);
    var ReadBuffer_Size = 1024;
    var FileStreamPos = 0;



    //Läser av hur många chunks som behöver skickas beroende på vad vi sätter för storleksgräns.
    function theSizeOfChunks(TargetFile) {
        var file = TargetFile[0];
        var Size = file.size;
        var MaxFileSizeMB = 1;
        var BufferChunkSize = MaxFileSizeMB * (1024 * 1024);
        var TotalParts = Math.ceil(Size / BufferChunkSize);
        return TotalParts;
    }
    //Läser av storleken på filen
    function theSizeOfFile(TargetFile) {
        console.log(TargetFile);
        var theSize = TargetFile[0].size;        
        return theSize;
    }

    //Läser checksum för filen som sedan skickas iväg som metadata.
    function GetSHA1(TargetFile, CarryOnCallback) {
        console.log(TargetFile);
        var file = TargetFile[0];
        var reader = new FileReader();
        reader.onload = function (event) {
            var binary = event.target.result;
            var hashCode = $.sha1(binary);
            CarryOnCallback(hashCode);
        };
        reader.readAsArrayBuffer(file);
        console.log("Loading");
    }

    //Får filändelsen av den valda filen, ex .pdf, .exe, .jpg..
    function GetFileExtension(filename) {
        return filename.split('.').pop();
    }

    //Exekveras när man trycker på upload
    $('#engage').click(function () {
        console.log("hej");
        TargetFile = $('#selectedfile')[0].files;
        fileinfo = $('#selectedfile')[0].files[0];
        filename = fileinfo.name;
        Checksum = GetSHA1(TargetFile, CarryOn);
    });

    //Funktionen kallas på när checksum är OK, skapar ett objekt av fil-elementen.
    function CarryOn(hashCode) {
        
        ObjectElement.id = Id;        
        ObjectElement.checksum = hashCode;        
        ObjectElement.fileName = filename;
        ObjectElement.uploaded = Uploaded;
        if (document.getElementById("publ").checked == true) {
            Private = false;
            ObjectElement.private = Private;
        }
        else {
            ObjectElement.private = Private;
        }
        Datatype = GetFileExtension(filename);
        ObjectElement.dataType = Datatype;
        Filesize = theSizeOfFile(TargetFile);
        console.log(Filesize);
        ObjectElement.fileSize = Filesize;
        ObjectElement.userId = Userid;
        DataChunks = null;
        ObjectElement.dataChunks = DataChunks;
        theInput = JSON.stringify(ObjectElement);
        console.log(theInput);
        
        SendData(theInput);
        return false;
    };


    //Här skickar vi metadatan och får tillbaks data.
    function SendData(theInput) {

        var FD = theInput;
        $.ajax({
            type: "POST",
            //url: 'http://localhost:56875/api/v1.0/FileItems/',
            url: '../api/v1.0/FileItems/',
            contentType: 'application/json',
            dataType: 'json',
            data: FD,
            error: function (e) {
                console.log(e);
            },
            //Är det ok, så påbörjar vi metoden med att skicka datachunks av filen.
            success: function (result, status, jqHXR) {
                var jsonUpdateData = result;
                Datatype: "json",
                console.log("Första");
                UploadFile(jsonUpdateData, TargetFile);
            }
        });
    };

    //Delar upp filen, namnger den och skickar den vidare-
    //till reader för att få de sista elementen innan det skickas.
    function UploadFile(jsonUpdateData, TargetFile) {
        var id;
        var FileID = jsonUpdateData.id;
        console.log("!!!!!!!!!!!!!!!!!!!!!!")
        console.log(FileID);
        console.log("!!!!!!!!!!!!!!!!!!!!!!")
        var fileitemlist = null;
        var file = TargetFile[0];
        var EndPos = BufferChunkSize;
        var Size = file.size;
        
        while (FileStreamPos < Size) {
            FileChunk.push(file.slice(FileStreamPos, EndPos));
            FileStreamPos = EndPos; // Hoppar för varje läst fil.
            EndPos = FileStreamPos + BufferChunkSize; // sätter nästa chunk-längd.
            
        }

        var TotalParts = FileChunk.length;
        var PartCount = 0;

        while (chunk = FileChunk.shift()) {
            console.log("BEGINS");
            
            var blob = new Blob([chunk], { type: 'application/octet-binary' });
            //console.log(blob);
            console.log("ENDS!");
            var promise = new Promise(ReadingTheBytesAndCheckSum);
            console.log("foreachsecond");
            promise.then(function (data) {
                PartCount++;
                
                var FilePartName = file.name + ".part_" + PartCount + "." + TotalParts;
                var byteData = data.byteArray;
                
                console.log("????????");
                console.log(byteData);
                console.log("????????");
                //ChunkElement.Id = id;
                //ChunkElement.CheckSum = data.hashCode2;
                //ChunkElement.PartName = FilePartName;
                //ChunkElement.Data = byteData;
                //ChunkElement.FileItemId = FileID;
                //ChunkElement.FileItem = fileitemlist;
                //console.log(ChunkElement);
                //skickar som ett json-objekt
                //var theInput = ChunkElement.serializeArray()
                //console.log(theInput);
                
                //for (var x in ChunkElement) {
                //    FD.append(x, ChunkElement[x]);
                //}                             
                var FD = new FormData();
                FD.append('Id', id);
                FD.append('PartName', FilePartName);
                FD.append('Checksum', data.hashCode2);
                
                var ali = 1234;
                // bblob = new Blob([byteData], { type: 'application/octet-binary' });
                FD.append('Data', ali);
                //FD.append('Data', bblob );
                //for (var i = 0; i < byteData.length; i++) {
                //    FD.append('Data[]', byteData[i]);
                //}
                FD.append('FileItemId', FileID);
                console.log(FD);
                
                $.ajax({
                    type: "POST",
                    //url: 'http://localhost:56875/api/v1.0/FileItems/' + FileID,
                    url: '../api/v1.0/FileItems/' + FileID,
                    //contentType: 'multipart/form-data',
                    contentType: false,
                    //dataType: 'json',
                    //datatype: false,
                    processData: false,
                    data: FD,
                    error: function (e) {
                        console.log(e);
                    },
                    success: function (result, status, jqHXR) {
                        var jsonUpdateData = result;
                        //Datatype: "json";
                        Datatype: false;
                    }
                })
                console.log("Done");
            });
            //Läser checksum och binär data för chunken
            function ReadingTheBytesAndCheckSum(resolve) {
                
                var reader = new FileReader();
                reader.onload = function (event) {                    
                    var binary = event.target.result;
                    var bytes = new Uint8Array(binary);
                    var byteArray = [].slice.call(bytes);                    
                    var hashCode2 = $.sha1(binary);
                    var theObject = { byteArray, hashCode2};                    
                    resolve(theObject);
                }
                reader.readAsArrayBuffer(blob);
            }
        }

    };
});







