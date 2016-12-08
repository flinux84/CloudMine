$(document).ready(function () {
    var UserIsSignIn = false;
    var userName = "";
    var userPassword = "";
    var userSecondPassword = "";
    var message = "";
    var $btn;

    // validera mail
    function validateEmail(email) {
        var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(email);
    }

    // validera password
    function validatePassWord(password) {
        var re = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{8,}/;
        return re.test(password);
    }

    // just for testing 
    function UseAjaxGetToken() {
        $.ajax({                                                               
            url: "../api/TestAuth",
            contentType: 'application/json',
            error: function (e) {
                console.log(e);
            },
            success: function (result, status) {
                console.log(result);
                console.log(status);
            }
        });
    }
    $("#getToken").click(UseAjaxGetToken);
    // just for testing end

    // Knapp för att logga in
    $('#idLogInButton').click(function () {
        $('#overlay').fadeIn(200, function () {
            $('#box').animate({ 'top': '200px' }, 200);
        });
        return false;
    });

    // knapp för att logga ut
    $('#idSignOutButton').click(function () {

        // kolla att användaren verkligen är inloggad, innan ajax 
        if (userName !== "") {
            userSignOut();
        }
    });
    function userSignOut() {
        $.ajax({
            //TODO: "https://localhost:44336/api/v1.0/Users/Logout"                                       <------adress-----<<<
            url: "../api/v1.0/Users/Logout",
            contentType: 'application/json',

            error: function (e) {
                console.log("error sign out: " + e);
                UserIsSignIn = true;
            },
            success: function (result, status) {
                console.log("success signout: " + result)
                UserIsSignIn = false;
            }
        });
        //.done(function () {
        //    UserSignOutStatus();
        //});
        // tvek att egentligen vänta på ajax
        UserIsSignIn = false;
        UserSignOutStatus();
    }
    function UserSignOutStatus() {
        if (!UserIsSignIn) {
            $("#idSignOutButton").addClass("hidden");
            $("#idLogInButton").removeClass("hidden");
            $(".registerUser").removeClass("hidden");
        }
    }

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
        //få submittknapp att snurra
        $btn = $(this).button('loading')
        // ta ut inputdata
        userName = document.forms["Loginform"]["mail"].value;
        userPassword = document.forms["Loginform"]["pword"].value;

        // kolla så mail är ok
        if (validateEmail(userName)) {
            // kolla så lösen är ok
            if (validatePassWord(userPassword)) {

                ajaxSignInUser(userName, userPassword);

            } else {
                message = "password is not valid";
                UserIsSignIn = false;
                UserSigninStatus();
                return false;
            }
        }
        else {
            message = "email is not correct.";
            UserIsSignIn = false;
            UserSigninStatus();
            return false;
        }
    });
    function ajaxSignInUser(userName, userPassword) {
        $.ajax({
            type: "POST",
            // TODO:     https://localhost:44336/token                                          // <------adress-----<<<
            url: '../token',
            contentType: 'application/x-www-form-urlencoded',
            data: { "username": userName, "password": userPassword },
            dataType: 'json',

            error: function (e) {
                UserIsSignIn = false;
                console.log("ajax call error " + e);
                message = "ajax call error. ";
                UserSigninStatus();
            },
            success: function (result, status) {
                console.log("ajax call - success")
                console.log(result);
                UserIsSignIn = true;
            }
        }).done(function () {
            UserSigninStatus();
        });
    }

    function UserSigninStatus() {
        // få knapp att sluta snurra
        $btn.button('reset')

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
    }

    // submit-funktion för regristrering
    $('form#Registerform').on('submit', function (e) {
        e.preventDefault();
        // få knapp att snurra  
        $btn = $(this).button('loading')

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

                    var ObjectElement = {};
                    ObjectElement.Email = userName;
                    ObjectElement.Password = userPassword;
                    var theInput = JSON.stringify(ObjectElement);

                    ajaxRegisterUser(userName, userPassword, theInput);

                } else {
                    message = "password is not valid.";
                    UserIsSignIn = false;
                    UserRegisterStatus();
                    return false;
                }
            } else {
                message = "password does not match.";
                UserIsSignIn = false;
                UserRegisterStatus();
                return false;
            }
        } else {
            message = "email is not correct.";
            UserIsSignIn = false;
            UserRegisterStatus();
            return false;
        }
    });
    function ajaxRegisterUser(userName, userPassword, theInput) {
        $.ajax({
            type: "POST",
            // TODO: https://localhost:44336/api/v1.0/Users                                   <------adress-----<<<
            url: '../api/v1.0/Users',
            contentType: 'application/json',
            data: theInput,
            dataType: 'json',

            error: function (e) {
                console.log("Error register: " + e);
                message = "Error register.";
                UserIsSignIn = false;
                UserRegisterStatus();
            },
            success: function (result, status) {
                console.log(result);
                UserIsSignIn = true;
            }
        }).done(function () {
            userSignInAfterRegister(userName, userPassword);

        });
    }

    function userSignInAfterRegister(userName, userPassword) {
        $.ajax({
            type: "POST",
            // TODO:   https://localhost:44336/token                                       <------adress-----<<<
            url: '../token',
            contentType: 'application/x-www-form-urlencoded',
            data: { "username": userName, "password": userPassword },
            dataType: 'json',

            error: function (e) {
                UserIsSignIn = false;
                console.log("ajax call - error sign in after register" + e);
                message = "ajax call - error sign in after register. ";
                UserRegisterStatus();
            },
            success: function (result, status) {
                console.log(result);
            }
        }).done(function () {
            UserRegisterStatus();
        });
    }

    function UserRegisterStatus() {
        // få knapp att sluta snurra
        $btn.button('reset')

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
    }

});