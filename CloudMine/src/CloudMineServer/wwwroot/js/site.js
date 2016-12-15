var progressFileLabel;
var progressBar;
var progressDiv;
var filetable;
var append;
var uploader;
var probar;
var editFileItem;
var RemoveFieldsInForm;

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
        } else {
            console.log("sign in to upload!");
        }
    })

    //list all files
    GetFileItems();

    //Create edit-dialog and auto-hide it
    var dialog = $("#edit-dialog").dialog({
        autoOpen: false,
        height: 400,
        width: 400,
        modal: true,
        resizable: false,
        buttons: {
            "Update": updateFileItem,
            Cancel: function () {
                dialog.dialog("close");
                RemoveFieldsInForm();
            }
        },
        close: function () {
            RemoveFieldsInForm();
        }
    });

    RemoveFieldsInForm = function() {
        $('#edit-form').children().remove();
    }

    function updateFileItem() {
        PutFileItem($('#edit-id').val());
    }
});
