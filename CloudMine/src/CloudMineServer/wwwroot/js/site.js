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
var headerToSort;
var Pagesize;

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
    headerToSort = 'id';
    Pagesize = $('#inputpagesize').val();

    //create progressbar
    probar = new ProgressBar(progressDiv, progressBar, progressFileLabel);

    //enable drag and drop functionality
    dragAndDrop(dropzone, probar);


    //setup fileuploader.js
    uploader = new TheFileUploader(probar);

    //create html-appender
    append = new HTMLappender(filetable);

    //list all files when page loads.
    GetFileItems('../api/v1.0/FileItems?pageNo=1&pageSize=' + Pagesize);

    //upload a file
    uploadform.change(function () {
        if (UserIsSignIn) {
            var fid = uploader.Upload(uploadform[0].files[0]);
        } else {
            console.log("sign in to upload!");
        }
    })

    //Pagesize change event
    $('#inputpagesize').change(function () {
        Pagesize = $(this).val();
        GetFileItems('../api/v1.0/FileItems?pageNo=1&pageSize=' + Pagesize);
    })


    //Search-button click-event
    buttonSearch.click(function () {
        if (UserIsSignIn) {
            console.log(searchString.val());
            if (!searchString.val() == "") {
                var sortUrl = "../api/v1.0/FileItems?filename=";
                sortUrl = sortUrl.concat(searchString.val());
                GetFileItems(sortUrl);
            } else {
                GetFileItems("../api/v1.0/FileItems");
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

            var sort = "?sort=";
            var order = "&order=";
            var pageNo = "&pageNo=";
            var sortUrl = "../api/v1.0/FileItems";
            var size = '&pageSize=';

            if (thisIDsorting == this.id) {
                console.log("går in i asc desc");
                if (sortAscending) {
                    sortAscending = false;
                    ascOrDesc = "desc"
                }
                else {
                    sortAscending = true;
                    ascOrDesc = "asc"
                }
            }
            else {
                sortAscending = false;
                ascOrDesc = "desc"
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
            //pageNo = pageNo.concat(currentPageIndex); //Currentpage.. Tror det är denna variabeln
            pageNo = pageNo.concat(Currentpage);
            size = size.concat(Pagesize);
            sortUrl = sortUrl.concat(sort, order, pageNo, size);

            GetFileItems(sortUrl);

        } else {
            console.log("sign in to sort!");
        }
    })


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
