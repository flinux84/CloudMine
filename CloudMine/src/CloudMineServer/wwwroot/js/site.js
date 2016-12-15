var progressFileLabel;
var progressBar;
var progressDiv;
var filetable;
var append;
var uploader;
var probar;

$(document).ready(function () {
    dropzone = $("#dropzone");
    uploadform = $("#uploadFile");
    progressDiv = $("#progressDiv");
    progressBar = $("#progressBar");
    progressFileLabel = $("#filelabel");
    filetable = $("#filetable");

    //create progressbar
    probar = new ProgressBar(progressDiv, progressBar, progressFileLabel);

    //enable drag and drop functionality
    dragAndDrop(dropzone, probar);


    //setup fileuploader.js
    uploader = new TheFileUploader(probar);

    //create html-appender
    append = new HTMLappender(filetable);

    //upload a file
    uploadform.change(function () {
        if (UserIsSignIn) {
        var fid = uploader.Upload(uploadform[0].files[0]);
        //GetFileItem(fid);
        } else {
            console.log("sign in to upload!");
        }
    })

    //list all files
    GetFileItems();

    ////list specific file
    //function listNewFileItem(fileitemId) {
    //    $.ajax({
    //        type: "GET",
    //        url: '../api/v1.0/FileItems/' + fileitemId
    //    }).done(function (result) {
    //        append.appendTable(result);
    //        Datatype: "json";
    //    }).fail(function (e) {
    //        console.log(e);
    //    })
    //}

    //function DeleteFileItem(fileitemId) {
    //    console.log("delete " + fileitemId);
    //}


});
