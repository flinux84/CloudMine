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
    uploadbutton = $("upload");
    progressDiv = $("#progressDiv");
    progressBar = $("#progressBar");
    progressFileLabel = $("#filelabel");
    filetable = $("#filetable");

    //enable drag and drop functionality
    dragAndDrop(dropzone);

    //create progressbar (and do a testrun)
    var probar = new ProgressBar(progressDiv, progressBar, progressFileLabel);
    probar.DoATestRun();

    //var fileuploader = new fileuploader(probar, targetfile[]);

    //create html-appender
    var append = new HTMLappender(filetable);

    $.ajax({
        type: "GET",
        url: 'http://localhost:1234/api/v1.0/FileItems/',
        error: function (e) {
            console.log(e);
        },
        success: function (result) {
            append.appendTable(result);
            Datatype: "json";
        }
    })

});
