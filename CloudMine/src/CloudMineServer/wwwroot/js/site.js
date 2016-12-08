var progressFileLabel;
var uploadbutton;
var uploadzone;
var progressBar;
var progressDiv;


$(document).ready(function () {
    dropzone = $("#dropzone");
    uploadbutton = $("upload");
    progressDiv = $("#progressDiv");
    progressBar = $("#progressBar");
    progressFileLabel = $("#filelabel");

    //enable drag and drop functionality
    dragAndDrop(dropzone);

    var probar = new ProgressBar(progressDiv, progressBar, progressFileLabel);
    probar.DoATestRun();

    //progressbar skickas sedan in i fileuploader som inparameter och kan anropas med en int i procent samt filnamnet.

    //var fileuploader = new fileuploader(probar, targetfile[]);

});
