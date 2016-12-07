var progressFileLabel;
var progresslabel;
var uploadbutton;
var uploadzone;

$(document).ready(function () {
    dropzone = $("#dropzone");
    uploadbutton = $("upload");
    dragAndDrop(dropzone);
    progressDiv = $("#progressDiv");
    progressBar = $("progress-bar");
    progressFileLabel = $("#filelabel");
    var status = createProgressbar(progressDiv, progressBar, progressFileLabel);
    console.log = "banan";
    //status.finished();
    //status.updateProgress(50, "banan.jpg");
});
