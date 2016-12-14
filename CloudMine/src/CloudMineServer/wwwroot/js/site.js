var progressFileLabel;
var uploadbutton;
var uploadzone;
var progressBar;
var progressDiv;
var arrayOfFileItems;
var filetable;
var append;


$(document).ready(function () {
    dropzone = $("#dropzone");
    uploadform = $("#uploadFile");
    progressDiv = $("#progressDiv");
    progressBar = $("#progressBar");
    progressFileLabel = $("#filelabel");
    filetable = $("#filetable");

    //create progressbar
    var probar = new ProgressBar(progressDiv, progressBar, progressFileLabel);

    //enable drag and drop functionality
    dragAndDrop(dropzone, probar);


    //setup fileuploader.js
    var newloader = new TheFileUploader(probar);

    uploadform.change(function () {
        newloader.Upload(uploadform[0].files[0])
    })

    //create html-appender
    var append = new HTMLappender(filetable);

    //list all files
    $.ajax({
        type: "GET",
        url: '../api/v1.0/FileItems/',
        error: function (e) {
            console.log(e);
        },
        success: function (result) {
            append.appendTable(result);
            Datatype: "json";
        }
    })
});
