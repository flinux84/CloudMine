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

//Get massa filer, behöver massivt ändras så den tar inparametrar och kan använda alla funktioner i API'et med sök/sortering/paging
function GetFileItems() {
    ShowLoading();
    $.ajax({
        type: "GET",
        url: '../api/v1.0/FileItems/',
    }).done(function (result, status, jqXHR) {
        var headerInfo = $.parseJSON(jqXHR.getResponseHeader("X-PageInfo"));
        BuildPaging(headerInfo);
        Datatype: "json";
        append.appendTable(result);
    }).fail(function (e) {
        console.log(e);
    }).always(function () {
        HideLoading();
    })
}

function GetSortedFileItemsList(sortUrl) {
    ShowLoading();
    $.ajax({
        type: "GET",
        url: sortUrl,
    }).done(function (result, status, jqXHR) {
        var headerInfo = $.parseJSON(jqXHR.getResponseHeader("X-PageInfo"));
        BuildPaging(headerInfo);
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

// Clear out the list of files
function ClearDataTable() {
    $("tbody").replaceWith("<tbody></tbody>");
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

function BuildPaging(pagingInfo) {
    var ul = $('.pagination');
    ul.children().remove();

    //Previous button 
    if (pagingInfo.prevPageLink !== "") {
        ul.append('<li><a href="#" id="prevPageLink">Previous page</a></li>');
    }

    //Individual buttons for each page 
    if (pagingInfo.totalPages > 1) {
        for (var i = 0; i < pagingInfo.totalPages; i++) {
            ul.append('<li><a href="#" class="pageIndex">'+(i+1)+'</a></li>');
        }
    }

    //Next button
    if (pagingInfo.nextPageLink !== "") {
        ul.append('<li><a href="#" id="nextPageLink">Next page</a></li>');
    }

    //Click events for Next and Previous Buttons
    $('#nextPageLink, #prevPageLink').click(function (e) {
        e.preventDefault;
        if ($(this).is('#prevPageLink'))
            GetSortedFileItemsList(pagingInfo.prevPageLink);
        else
            GetSortedFileItemsList(pagingInfo.nextPageLink);
    });

    //Click events for individual page buttons
    $('.pageIndex').click(function (e) {
        e.preventDefault;
        GetSortedFileItemsList('/api/v1.0/FileItems?pageNo=' + $(this).text());
    });
}
