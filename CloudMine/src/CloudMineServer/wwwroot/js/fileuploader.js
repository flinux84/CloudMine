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

    function GetFileExtension(filename) {
        return filename.split('.').pop();
    }

    $('#engage').click(function () {
        console.log("hej");
        TargetFile = $('#selectedfile')[0].files;
        fileinfo = $('#selectedfile')[0].files[0];
        filename = fileinfo.name;
        Checksum = GetSHA1(TargetFile, CarryOn);
    });

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


    //Här är det tänkt att skicka metadata på filen som "ska" laddas upp.
    function SendData(theInput) {

        var FD = theInput;
        $.ajax({
            type: "POST",
            url: 'http://localhost:56875/api/v1.0/FileItems/',
            contentType: 'application/json',
            dataType: 'json',
            data: FD,
            error: function (e) {
                console.log(e);
            },
            success: function (result, status, jqHXR) {
                var jsonUpdateData = result;
                Datatype: "json",
                console.log("Första");
                UploadFile(jsonUpdateData, TargetFile);
            }
        });
    };

    function UploadFile(jsonUpdateData, TargetFile) {
        var id;
        var FileID = jsonUpdateData.id;
        var fileitemlist = null;
        var file = TargetFile[0];
        var EndPos = BufferChunkSize;
        var Size = file.size;
        console.log("Andra");
        while (FileStreamPos < Size) {
            FileChunk.push(file.slice(FileStreamPos, EndPos));
            FileStreamPos = EndPos; // jump by the amount read
            EndPos = FileStreamPos + BufferChunkSize; // set next chunk length
            console.log("foreachFirst");
        }

        var TotalParts = FileChunk.length;
        var PartCount = 0;

        while (chunk = FileChunk.shift()) {

            var blob = new Blob([chunk]);
            var promise = new Promise(ReadingTheBytesAndCheckSum);
            console.log("foreachsecond");
            promise.then(function (data) {
                PartCount++;
                console.log("test");
                var FilePartName = file.name + ".part_" + PartCount + "." + TotalParts;
                var byteData = data.byteArray;
                console.log(byteData);
                ChunkElement.Id = id;
                ChunkElement.CheckSum = data.hashCode2;
                ChunkElement.PartName = FilePartName;
                ChunkElement.Data = byteData;
                ChunkElement.FileItemId = FileID;
                ChunkElement.FileItem = fileitemlist;
                console.log(ChunkElement);
                var theChunkFile = JSON.stringify(ChunkElement);
                console.log("almost");
                $.ajax({
                    type: "POST",
                    url: 'http://localhost:56875/api/v1.0/FileItems/' + FileID,
                    contentType: 'application/json',
                    dataType: 'json',
                    data: theChunkFile,
                    error: function (e) {
                        console.log(e);
                    },
                    success: function (result, status, jqHXR) {
                        var jsonUpdateData = result;
                        Datatype: "json";
                    }
                })
                console.log("Done");
            });
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







