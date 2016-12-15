var progressFileLabel;
var progressBar;
var progressDiv;
var filetable;
var append;
var uploader;
var probar;
var editFileItem;
var RemoveFieldsInForm;
var sortAscending;

$(document).ready(function () {
    dropzone = $("#dropzone");
    uploadform = $("#uploadFile");
    progressDiv = $("#progressDiv");
    progressBar = $("#progressBar");
    progressFileLabel = $("#filelabel");
    filetable = $("#filetable");
    sortAscending = true;
    
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

    //Sort List
    $(".orderFileList").click(function () {
        if (UserIsSignIn) {

            var sort = "?sort=";
            var order = "&order=";
            var sortUrl = "../api/v1.0/FileItems";
         
            if (sortAscending)
            {
                sortAscending = false;
                order = order.concat("asc");
            }
            else {
                sortAscending = true;
                order = order.concat("desc");
            }

            switch (this.id) {
                case "orderName":
                    sort = sort.concat("FileName")
                    break;
                case "orderSize":
                    sort = sort.concat("FileSize")
                    break;
                case "orderDate":
                    sort = sort.concat("Uploaded")
                    break;
                case "orderType":
                    sort = sort.concat("DataType")
                    break;
                case "orderDescription":
                    sort = sort.concat("Description")
                    break;
                default:
                    sort = sort.concat("id")
            }

            sortUrl = sortUrl.concat(sort, order);

            GetSortedFileItemsList(sortUrl);
        } else {
            console.log("sign in to sort!");
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
