

//får filändelsen txt, jpeg, osv
function GetFileExtension(filename) {    
    return filename.split('.').pop();
}

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
    var file = TargetFile[0];
    var Size = file.size;
    return Size;
}


function GetSHA1(theTarget, CarryOnCallback) {
    console.log(theTarget);
    var file = theTarget[0];    
    var reader = new FileReader();    
    reader.onload = function (event) {
        var binary = event.target.result;        
        var hashCode = $.sha1(binary);
        CarryOnCallback(hashCode);
        //CarryOn(hashCode);
        //console.log("here it goes");
        //GetAsByte(theTarget);

    };
    reader.readAsArrayBuffer(file);    
    console.log("Loading");
}



//function ConvertToByte(chunk) {

//    var file = chunk;    
//    var blob = new Blob([file]);
//    var reader = new FileReader();
//    reader.onload = function(event){
//        var binary = event.target.result;
//        var bytes = new Uint8Array(binary);     
//        catchit = bytes;
//        console.log(catchit);
//    };
//    reader.readAsArrayBuffer(blob);
//}


jQuery.sha1 = sha1;
var Id;
var Checksum;
var FileName;
var Description;
var Uploaded = GetTime();
var Private = true;
var Datatype;
var Filesize;
var Userid = null;
var DataChunks;
var ObjectInfo = [];
var ObjectElement = {};
var theInput;
var theFile;

//Tänkte först serialisera hela formuläret med en gång, men det blir knas om man sedan ska ange nya värden till nya objekt, använder därför getElementById




$(document).ready(function () {
    $('#engage').click(function () {
        Checksum = GetSHA1($('#selectedfile')[0].files, CarryOn);
    });
});

function CarryOn(hashCode) {
    //Checksum = $.sha1($('#selectedfile')[0].files);    
    //var hej = $.sha1("apa");    
    //Checksum = "1517f9ff-62c2-4b3b-98ec-9d3a0abd63cd";
    //Checksum = null;
    ObjectElement.id = Id;
    Checksum = hashCode;
    //console.log("exec");
    ObjectElement.checksum = Checksum;
    FileName = document.getElementById("FileName").value;
    ObjectElement.fileName = FileName;
    //Description = document.getElementById("description").value;
    //ObjectElement.description = Description;
    ObjectElement.uploaded = Uploaded;
    if (document.getElementById("publ").checked == true) {
        Private = false;
        ObjectElement.private = Private;
    }
    else {
        ObjectElement.private = Private;
    }
    Datatype = GetFileExtension();
    ObjectElement.dataType = Datatype;
    Filesize = theSizeOfFile($('#selectedfile')[0].files);
    ObjectElement.fileSize = Filesize;
    ObjectElement.userId = Userid;
    //DataChunks = theSizeOfChunks($('#selectedfile')[0].files);
    
    DataChunks = null;
    ObjectElement.dataChunks = DataChunks;
    theInput = JSON.stringify(ObjectElement);
    console.log(theInput);
    theFile = $('#selectedfile')[0].files;
    SendData(theInput);
    //Uint8Array

    return false;
};


//Här är det tänkt att skicka metadata på filen som "ska" laddas upp.
 function SendData(theInput) {
    //console.log(theInput);
    //var FD = new FormData();
    //FD.append(ObjectInfo);
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
            
            
            //SliceAndDiceThemFiles(($('#selectedfile')[0].files));
            //console.log(jsonUpdateData);
            UploadFile(jsonUpdateData, theFile);
        }

});

};

 var ChunkElement= {};
 

 function UploadFile(TargetFile, theFile) {
     var id;
     var FileID = TargetFile.id;     
     var fileitemlist = null;

    
    
    var FileChunk = [];
    var file = theFile[0];
       
    
    var MaxFileSizeMB = 1;
    var BufferChunkSize = MaxFileSizeMB * (1024 * 1024);
    var ReadBuffer_Size = 1024;
    var FileStreamPos = 0;
    var EndPos = BufferChunkSize;
    //var Size = Filesize;
    var Size = file.size;

    while (FileStreamPos < Size) {
        FileChunk.push(file.slice(FileStreamPos, EndPos));
        FileStreamPos = EndPos; // jump by the amount read
        EndPos = FileStreamPos + BufferChunkSize; // set next chunk length
    }

    var TotalParts = FileChunk.length;
    var PartCount = 0;
    
        while (chunk = FileChunk.shift()) {
            
            //ConvertToByte(chunk)            
            var blob = new Blob([chunk]);

            var promise = new Promise(ReadingTheBytesAndCheckSum);

            promise.then(function (data) {
                console.log(data.byteArray);
                console.log(data.hashCode2);
                
                PartCount++;
                var FilePartName = file.name + ".part_" + PartCount + "." + TotalParts;
                //console.log(data);
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
                //console.log(theChunkFile);

                $.ajax({
                    type: "POST",        
                    url: 'http://localhost:56875/api/v1.0/FileItems/'+ FileID,
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
                    var theObject = {byteArray,hashCode2};
                    //console.log(hashCode2);
                    resolve(theObject);



                };
                reader.readAsArrayBuffer(blob);
            }
            //Uploadz(chunk, FilePartName);
        }
 }




 


