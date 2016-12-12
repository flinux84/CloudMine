var progressFileLabel;
var uploadbutton;
var uploadzone;
var progressBar;
var progressDiv;
var arrayOfFileItems;
var filetable;
var append;
var sha;

$(document).ready(function () {
    dropzone = $("#dropzone");
    uploadform = $("#uploadFile");
    progressDiv = $("#progressDiv");
    progressBar = $("#progressBar");
    progressFileLabel = $("#filelabel");
    filetable = $("#filetable");
    jQuery.sha1 = sha;
    //enable drag and drop functionality
    dragAndDrop(dropzone);

    //create progressbar (and do a testrun)
    var probar = new ProgressBar(progressDiv, progressBar, progressFileLabel);
    probar.DoATestRun();

    //setup fileuploader.js
    //var uploader = new Uploader(uploadform, sha);
    //var fileuploader = new fileuploader(probar, targetfile[]);

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
