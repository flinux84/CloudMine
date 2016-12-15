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


//Put todo