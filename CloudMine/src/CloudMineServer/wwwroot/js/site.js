var progressFileLabel;
var progressBar;
var progressDiv;
var filetable;
var append;
var uploader;
var probar;
var searchString;
var buttonSearch;
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
    searchString = $("#searchString");
    buttonSearch = $("#buttonSearch")
    sortAscending = true;
    var thisIDsorting = "";
    var currentPageIndex = "1";
    var ascOrDesc = "";
    var headerToSort = "";

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

    buttonSearch.click(function () {
        if (UserIsSignIn) {
            console.log(searchString.val());
            if (!searchString.val() == "") {
                var sortUrl = "../api/v1.0/FileItems?filename=";
                sortUrl = sortUrl.concat(searchString.val());
                GetSortedFileItemsList(sortUrl);
            } else {
                GetSortedFileItemsList("../api/v1.0/FileItems");
                // GetFileItems();
            }
        } else {
            console.log("sign in to search");
        }

    });

    //Sort List
    $(".orderFileList").click(function () {
        if (UserIsSignIn) {

            var currentElementId = this.id;
            var pagePush = currentElementId.startsWith("p");

            var sort = "?sort=";
            var order = "&order=";
            var pageNo = "&pageNo=";
            var sortUrl = "../api/v1.0/FileItems";

            if (pagePush) {
                console.log("page push");
                console.log(this.id);
                currentPageIndex = currentElementId.slice(-1);
                console.log("current page index: " + currentPageIndex);
            }

            if (thisIDsorting == this.id && !pagePush) {
                console.log("går in i asc desc");
                if (sortAscending) {
                    sortAscending = false;
                    ascOrDesc = "asc"
                }
                else {
                    sortAscending = true;
                    ascOrDesc = "desc"
                }
            }
            else {
                ascOrDesc = "asc"
                sortAscending = false;
            }

            switch (this.id) {
                case "orderName":
                    headerToSort = "FileName";
                    thisIDsorting = this.id;
                    break;
                case "orderSize":
                    headerToSort = "FileSize";
                    thisIDsorting = this.id;
                    break;
                case "orderDate":
                    headerToSort = "Uploaded";
                    thisIDsorting = this.id;
                    break;
                case "orderType":
                    headerToSort = "DataType";
                    thisIDsorting = this.id;
                    break;
                case "orderDescription":
                    headerToSort = "Description";
                    thisIDsorting = this.id;
                    break;
                default:
                    headerToSort = "id";
                    thisIDsorting = this.id;
            }

            sort = sort.concat(headerToSort)
            order = order.concat(ascOrDesc);
            pageNo = pageNo.concat(currentPageIndex);
            sortUrl = sortUrl.concat(sort, order, pageNo);

            GetSortedFileItemsList(sortUrl);

        } else {
            console.log("sign in to sort!");
        }
    })

    //list all files
    GetFileItems();

    //Create edit-dialog and auto-hide it
    var dialog = $("#edit-dialog").dialog({
        classes: { 'ui-dialog-titlebar-close': 'hidden' },
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

    RemoveFieldsInForm = function () {
        $('#edit-form').children().remove();
    }

    function updateFileItem() {
        PutFileItem($('#edit-id').val());
    }
});
