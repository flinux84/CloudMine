//// Write your Javascript code.
//function UploadFile(TargetFile) {

//    var FileChunk = [];
//    var file = TargetFile[0];
//    var MaxFileSizeMB = 10;
//    var BufferChunkSize = MaxFileSizeMB * (1024 * 1024);
//    var ReadBuffer_Size = 1024;
//    var FileStreamPos = 0;
//    var EndPos = BufferChunkSize;
//    var Size = file.size;

//    while (FileStreamPos < Size) {
//        FileChunk.push(file.slice(FileStreamPos, EndPos));
        
//        FileStreamPos = EndPos; // jump by the amount read
//        EndPos = FileStreamPos + BufferChunkSize; // set next chunk length
//    }

//    var TotalParts = FileChunk.length;
//    var PartCount = 0;

//    while (chunk = FileChunk.shift()) {
//        PartCount++;
//        var FilePartName = file.name + ".part_" + PartCount + "." + TotalParts;
//        UploadFileChunk(chunk, FilePartName);
//    }
//}




//var bar = new ProgressBar.Circle(progress, {
//    strokeWidth: 6,
//    easing: 'easeInOut',
//    duration: 1400,
//    color: 'lightblue',
//    trailColor: '#eee',
//    trailWidth: 1,
//    svgStyle: null
//});



function SliceAndDiceThemFiles(TargetFile) {
    
}

//Detta sätter filnamnet automatiskt när man väljer fil.
$("#selectedfile").change(function(e){
    $("#FileName").val($("#selectedfile").val().split('\\').pop().split('/').pop());
});

//Skapade denna för att sätta nullvärde på oskriven text i formuläret, tror inte vi kommer behöva denna.
$.fn.SerializeToJson = function () {
    var object = {};
    var array = this.serializeArray();
    $.each(array,function(){
        if (object[this.name] !== undefined)
        {
            if (!object[this.name].push)
            {
                object[this.name]= [object[this.name]];
            }
            object[this.name].push(this.value || null);
        }
        else
        {
            object[this.name] = this.value || null;
        }
    });
    return object;
};

//tidsstämpel då filen laddas upp.
function GetTime() {
    var time = new Date();
    time.getHours();
    time.getMinutes();
    time.getSeconds();
    return time;
};

//får filändelsen txt, jpeg, osv
function GetFileExtension() {
    var extension = document.getElementById("selectedfile").value;
    return extension.split('.').pop();
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

//Tänkte först serialisera hela formuläret med en gång, men det blir knas om man sedan ska ange nya värden till nya objekt, använder därför getElementById
var ObjectInfo = [];
var ObjectElement = {};
$(document).ready(function () {
    $('form').submit(function () {
                
        ObjectElement.id = Id;
        //Checksum = $.sha1($('#selectedfile')[0].files);
        Checksum = "1517f9ff-62c2-4b3b-98ec-9d3a0abd63cd";
        //Checksum = null;
        ObjectElement.checksum = Checksum;
        FileName = document.getElementById("FileName").value;
        ObjectElement.fileName = FileName;
        Description = document.getElementById("description").value;
        ObjectElement.description = Description;
        ObjectElement.uploaded = Uploaded;
        if (document.getElementById("publ").checked == true)
        {
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
        //console.log(DataChunks);
        DataChunks = null;
        ObjectElement.dataChunks = DataChunks;
        theInput = JSON.stringify(ObjectElement);
        console.log(theInput);
        SendData(theInput);
        //Uint8Array
                
        //console.log(theInput);
        //ObjectInfo.push(theInput);
        //console.log(ObjectInfo);
        //SendData(ObjectInfo);
        //location.reload();
        return false;
    })
});

//Här är det tänkt att skicka metadata av filen som "ska" laddas upp.
function SendData(theInput) {
    //var FD = new FormData();
    //FD.append(ObjectInfo);
    var FD = theInput;
    $.ajax({
        type: "POST",
        //url: 'http://localhost:57260/api/v1.0/FileItems/PostFileItem',
        url: 'http://localhost:65062/api/v1.0/FileItems/',
        contentType: 'application/json',
        
        dataType: 'json',
        //data: JSON.stringify(FD),
        data: FD,
        error: function (e) {
            console.log(e);
        },
        success: function (result, status, jqHXR) {
            var jsonUpdateData = result;
            Datatype: "json",
            console.log("Buddha")
            SliceAndDiceThemFiles(($('#selectedfile')[0].files));
            console.log(jsonUpdateData);
            //switch (result) {
            //    case true:
            //        processResponse(result);
            //        break;
            //    default:
            //        resultDiv.html(result);

        }

});

    };


//$(document).ready(bar.animate(1.0));
