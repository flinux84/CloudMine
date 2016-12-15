//ajax-anrop för varje API-metod så vi kan använda "globalt".

//Get Specific FileData
function GetFileItem(fileitemId) {
    $.ajax({
        type: "GET",
        url: '../api/v1.0/FileItems/' + fileitemId
    }).done(function (result) {
        Datatype: "json";
        console.log("test");
        console.log(result);
        append.appendTable(result);
        
    }).fail(function (e) {
        console.log(e);
    }).always(function () {
       
    })
}

//Get massa filer, behöver massivt ändras så den tar inparametrar och kan använda alla funktioner i API'et med sök/sortering/paging
function GetFileItems() {
    $.ajax({
        type: "GET",
        url: '../api/v1.0/FileItems/',
    }).done(function (result) {
        Datatype: "json";
        append.appendTable(result);
    }).fail(function (e) {
        console.log(e);
    }).always(function () {
      
    })
}

function GetSortedFileItemsList(sortUrl) {
    $.ajax({
        type: "GET",
        url: sortUrl,
    }).done(function (result) {
        Datatype: "json";
        ClearDataTable();
        append.appendTable(result);
    }).fail(function (e) {
        console.log(e);
    }).always(function () {

    })
}

//Delete file
function DeleteFileItem(fileitemId) {
    console.log("trying to delete a file");
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
        //Todo: stuff
    })

}

// Clear out the list of files
function ClearDataTable() {
    $("tbody").replaceWith("<tbody></tbody>");
}


//Put todo
function PutFileItem(fileItemId) {
    $.getJSON('/api/v1.0/FileItems/' + fileItemId).done(function (response) {
        response.fileName = $('#edit-filename').val();
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
    append.replaceRow(updatedFileItem.id);
}
