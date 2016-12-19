//ajax-anrop för varje API-metod så vi kan använda "globalt".

//Get Specific FileData
function GetFileItem(fileitemId) {
    ShowLoading();
    $.ajax({
        type: "GET",
        url: '../api/v1.0/FileItems/' + fileitemId
    }).done(function (result) {
        Datatype: "json";
        console.log("Get FileItem Meta");
        console.log(result);
        append.addOrReplaceRow(result);
    }).fail(function (e) {
        console.log(e);
    }).always(function () {
        HideLoading();
    })
}

//Get lots of fileitems yeah baby
function GetFileItems(sortUrl) {
    ShowLoading();
    if (!sortUrl || 0 === sortUrl.length) {
        sortUrl = '../api/v1.0/FileItems/';
    }
    $.ajax({
        type: "GET",
        url: sortUrl
    }).done(function (result, status, jqXHR) {        
        var pagingInfo = $.parseJSON(jqXHR.getResponseHeader("X-PageInfo"));
        append.makePagination(pagingInfo)
        Datatype: "json";
        ClearDataTable();
        append.appendTable(result);
    }).fail(function (e) {
        console.log(e);
    }).always(function () {
        HideLoading();
    })
}

//Delete file
function DeleteFileItem(fileitemId) {
    console.log("trying to delete a file");
    ShowLoading();
    $.ajax({
        type: "DELETE",
        url: '../api/v1.0/FileItems/' + fileitemId,
    }).done(function (result) {
        Datatype: "json";
        console.log("successfully deleted a file, refresh")
        append.deleteRow(fileitemId)
    }).fail(function (e) {
        console.log(e);
    }).always(function () {
        HideLoading();
    })

}

function getUserAccountInfo() {
    $.ajax({
        url: '../api/v1.0/Users/UserInfo',
        contentType: 'application/json',
        success: function (result) {
// Clear out the list of files
function ClearDataTable() {
    $("tbody").replaceWith("<tbody></tbody>");
}

            console.log(result);
            Datatype: "json";
            append.userAccountInfo(result);
        },
        error: function (e) {
            console.log(e.status);
        }

    });
}




//Put todo
function PutFileItem(fileItemId) {
    $.getJSON('/api/v1.0/FileItems/' + fileItemId).done(function (response) {
        //response.fileName = $('#edit-filename').val();
        response.description = $('#edit-description').val();
        $.ajax({
            url: '/api/v1.0/FileItems/' + response.id,
            type: 'PUT',
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(response)
        }).done(CloseDialogAndUpdateRow(response));
    });    
}

function CloseDialogAndUpdateRow(updatedFileItem) {
    $("#edit-dialog").dialog("close");
    RemoveFieldsInForm();
    append.addOrReplaceRow(updatedFileItem);
}
