$(document).ready(function () {

    // not in use any more
    function UseAjaxGetToken() {
        console.log("this is just a empty button")
    }
    $("#getToken").click(UseAjaxGetToken);

    // register
    $('form#firstForm').submit(function (e) {
        e.preventDefault();
        console.log("first")


        var Email = document.forms["firstForm"]["mailx"].value;
        var Password = document.forms["firstForm"]["pwordx"].value;

        var ObjectElement = {};
        ObjectElement.Email = Email;
        ObjectElement.Password = Password;
        var theInput = JSON.stringify(ObjectElement);

        $.ajax({
            type: "POST",
            url: 'http://localhost:17881/api/v1.0/Users',
            contentType: 'application/json',
            data: theInput,
            dataType: 'json',

            error: function (e) {
                console.log(e);
            },
            success: function (result, status, jqHXR) {
                var jsonUpdateData = result;
                Datatype: "json",
                console.log(jsonUpdateData);
                console.log("just the token:");
                console.log(jsonUpdateData.access_token);

            }
        });

    })

    // get token
    $('form#secondForm').on('submit', function (e) {
        e.preventDefault();
        console.log("second")
        var usermail = document.forms["secondForm"]["mail"].value;
        var userpassword = document.forms["secondForm"]["pword"].value;

        console.log(usermail + " " + userpassword)

        $.ajax({
            type: "POST",
            url: 'http://localhost:17881/token',
            contentType: 'application/x-www-form-urlencoded',
            data: { "username": usermail, "password": userpassword },
            dataType: 'json',

            error: function (e) {
                console.log(e);
            },
            success: function (result, status, jqHXR) {
                var jsonUpdateData = result;
                Datatype: "json",
                console.log(jsonUpdateData);
                console.log("just the token:");
                console.log(jsonUpdateData.access_token);
                return jsonUpdateData.access_token;
            }
        });

    });

    // staic add fileitem
    $('form#thirdForm').on('submit', function (e) {
        e.preventDefault();
        console.log("third")
        var usermail = document.forms["secondForm"]["mail"].value;
        var userpassword = document.forms["secondForm"]["pword"].value;
        var ObjectElement = {};
        ObjectElement.Email = usermail;
        ObjectElement.Password = userpassword;
        var theInput = JSON.stringify(ObjectElement);

        //
        var FileItemElement = {};
        FileItemElement.id = 0,
        FileItemElement.checksum = "93657799-3937-48db-ab50-65d22be62732",
        FileItemElement.fileName = "staticApiFile",
        FileItemElement.description = "generate api call add static fileitem to db",
        FileItemElement.uploaded = "0001-01-01T00:00:00",
        FileItemElement.private = false,
        FileItemElement.dataType = "jpg",
        FileItemElement.fileSize = 88,
          FileItemElement.userId = null,
        //FileItemElement.userId = "b3295177-17a4-4df1-ad2b-c360eb87254a",
        FileItemElement.dataChunks = null
        var TheFileItemObj = JSON.stringify(FileItemElement);
        //

        $.ajax({
            type: "POST",
            url: 'http://localhost:17881/token',
            contentType: 'application/x-www-form-urlencoded',
            data: { "username": usermail, "password": userpassword },
            dataType: 'json',

            error: function (e) {
                console.log(e);
            },
            success: function (result, status, jqHXR) {
                console.log("sucess token n stuff")
                //

                $.ajax({
                    type: "POST",
                    url: "http://localhost:17881/api/v1.0/FileItems",
                    contentType: 'application/json',
                    data: TheFileItemObj,
                    dataType: 'json',

                    error: function (e) {
                        console.log(e);
                    },
                    success: function (result, status) {
                        console.log(result);
                        console.log(status);
                    }
                });

                //$.ajax({
                //    type: "POST",
                //    beforeSend: function (request) {
                //        request.setRequestHeader("Authority", result.access_token);
                //    },
                //    url: "http://localhost:17881/api/v1.0/FileItems",
                //    data: TheFileItemObj,

                //    error: function (e) {
                //        console.log(e);
                //    },
                //    success: function (result, status) {
                //        console.log(result);
                //        console.log(status);
                //    }
                //});
                //
            }
        });

    });

});