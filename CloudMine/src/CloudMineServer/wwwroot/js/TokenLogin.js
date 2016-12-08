var UserIsSignIn = false;
var userName = "";
var userPassword = "";
var userSecondPassword = "";
var message = "";

// känner tydligen av regex redan vid inmatningen. går inte submitta om reggen inte stämer. vilket gör mina felmeddelande lite överflödiga.. 
function validateEmail(email) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}

function validatePassWord(password) {
    var re = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{8,}/;
    return re.test(password);
}

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


    // Knapp för att logga in
    $('#idLogInButton').click(function () {
        $('#overlay').fadeIn(200, function () {
            $('#box').animate({ 'top': '200px' }, 200);
        });
        return false;
    });

    // knapp för att logga ut
    $('#idSignOutButton').click(function () {

        // TODO: ajax till serven och logga ut. Sätta värde på "UserIsSignIn"                       <-----------<<<

        UserIsSignIn = false;
        if (!UserIsSignIn) {
            $("#idSignOutButton").addClass("hidden");
            $("#idLogInButton").removeClass("hidden");
            $(".registerUser").removeClass("hidden");
        }
    });

    // knapp för att registrera sig
    $('.registerUser').click(function () {
        $('#overlay').fadeIn(200, function () {
            $('#boxErrorRegister').animate({ 'top': '-200px' }, 500);
            $('#boxRegister').animate({ 'top': '200px' }, 200);
        });
        return false;
    });

    // knapp för att öppna inloggning igen efter error att logga in
    $('.signinuseragain').click(function () {
        $('#overlay').fadeIn(200, function () {
            $('#boxErrorLogin').animate({ 'top': '-200px' }, 500);
            $('#box').animate({ 'top': '200px' }, 200);
        });
        return false;
    });

    // knapp för att kryssa inloggnings-lådan
    $('#boxclose').click(function () {
        $('#box').animate({ 'top': '-200px' }, 500, function () {
            $('#overlay').fadeOut('fast');
        });
    });
    // knapp för att kryssa regristrerings-lådan
    $('#boxRegisterclose').click(function () {
        $('#boxRegister').animate({ 'top': '-200px' }, 500, function () {
            $('#overlay').fadeOut('fast');
        });
    });
    // knapp för att kryssa regristrerings-error-lådan
    $('#boxErrorclose').click(function () {
        $('#boxErrorRegister').animate({ 'top': '-200px' }, 500, function () {
            $('#overlay').fadeOut('fast');
        });
    });
    // knapp för att kryssa inloggnings-error-lådan
    $('#boxErrorLoginclose').click(function () {
        $('#boxErrorLogin').animate({ 'top': '-200px' }, 500, function () {
            $('#overlay').fadeOut('fast');
        });
    });

    // submit-funktion för inloggning
    $('form#Loginform').on('submit', function (e) {
        e.preventDefault();
        // ta ut inputdata
        userName = document.forms["Loginform"]["mail"].value;
        userPassword = document.forms["Loginform"]["pword"].value;

        // kolla så mail är ok
        if (validateEmail(userName)) {
            // kolla så lösen är ok
            if (validatePassWord(userPassword)) {

                // TODO: ajax till serven och logga in. Sätta värde på "UserIsSignIn"                       <-----------<<<
                console.log("ajax call - sign in")

                message = "message if no good";
                UserIsSignIn = true;
            } else {
                message = "password is not valid";
                UserIsSignIn = false;
                return false;
            }
        }
        else {
            message = "email is not correct";
            UserIsSignIn = false;
            return false;
        }

        // valideringen har gått igenom
        if (UserIsSignIn) {
            $('#box').animate({ 'top': '-200px' }, 500, function () {
                $('#overlay').fadeOut('fast');
            });
            $("#idLogInButton").addClass("hidden");
            $(".registerUser").addClass("hidden");
            $("#idSignOutButton").removeClass("hidden");
            return false;
        }
            // nåt gick snett
        else if (!UserIsSignIn) {
            $(".result").text(message);
            $(".result").css("color", "red");
            $('#box').animate({ 'top': '-200px' }, 500, function () {
                $('#boxErrorLogin').animate({ 'top': '200px' }, 200);
            });
            return false;
        }
    });

    // submit-funktion för regristrering
    $('form#Registerform').on('submit', function (e) {
        e.preventDefault();
        // ta ut inputdata
        userName = document.forms["Registerform"]["mail"].value;
        userPassword = document.forms["Registerform"]["pword"].value;
        userSecondPassword = document.forms["Registerform"]["repword"].value;

        // kolla att användarnamn / mail är ok
        if (validateEmail(userName)) {
            // kolla att lösenord matchar
            if (userPassword === userSecondPassword) {
                // validera att lösenord är ok
                if (validatePassWord(userPassword)) {

                    // TODO: ajax till serven och logga in. Sätta värde på "UserIsSignIn"                       <-----------<<<
                    console.log("ajax call - register and sign in")

                    message = "message if no good";
                    UserIsSignIn = true;
                } else {
                    message = "password is not valid";
                    UserIsSignIn = false;
                    return false;
                }
            } else {
                message = "password does not match";
                UserIsSignIn = false;
                return false;
            }
        } else {
            message = "email is not correct";
            UserIsSignIn = false;
            return false;
        }

        if (UserIsSignIn) {
            $('#boxRegister').animate({ 'top': '-200px' }, 500, function () {
                $('#overlay').fadeOut('fast');
            });
            // ändra synliga divar
            $("#idLogInButton").addClass("hidden");
            $(".registerUser").addClass("hidden");
            $("#idSignOutButton").removeClass("hidden");
        } else if (!UserIsSignIn) {
            //sätt felmeddelande på error popup
            $(".result").text(message);
            $(".result").css("color", "red");
            $('#boxRegister').animate({ 'top': '-200px' }, 500, function () {
                $('#boxErrorRegister').animate({ 'top': '200px' }, 200);
            });
        }
    });
});