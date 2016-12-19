var UserIsSignIn = false;

$(document).ready(function () {

    /*JS Sign in*/

    var userName = "";
    var userPassword = "";
    var userSecondPassword = "";
    var message = "";
    var $btn;
    var userPushButtonToSignOut = false;

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
        console.log("knapp urkopplad");
    }
    $("#getToken").click(UseAjaxGetToken);
    // just for testing end

    // check user Authenticated-status. Kolla om användaren är inloggad när sidan laddas
    function AjaxUserIsLoggedIn() {
        $.ajax({
            //TODO: "https://localhost:44336/api/v1.0/Users/IsLoggedIn"                                       <------adress-----<<<
            url: "../api/v1.0/Users/IsLoggedIn",
            contentType: 'application/json',

        }).done(function (result) {
            console.log("is user signin: " + result)
            UserIsSignIn = result;
            getUserAccountInfo();
        })
  .fail(function () {
      console.log("error Authenticated check");
  })
  .always(function () {
      UserSignOutStatus();
  });

    }
    AjaxUserIsLoggedIn();

    // Knapp för att logga in
    //$('#idLogInButton').click(function () {
    $('.LogInButtonClass').click(function () {
        $("#box").removeClass("hidden");
        $('#overlay').fadeIn(200, function () {
            $('#box').animate({ 'top': '200px' }, 200);
        });
        return false;
    });

    // knapp för att logga ut
    //$('#idSignOutButton').click(function () {
    $('.SignOutButton').click(function () {
        userPushButtonToSignOut = true;
        userSignOut();
    });
    function userSignOut() {
        $.ajax({
            //  type: "POST",
            //TODO: "https://localhost:44336/api/v1.0/Users/Logout"                                       <------adress-----<<<
            url: "../api/v1.0/Users/Logout",
            contentType: 'application/json',

        }).done(function () {
            console.log("signout done")
            UserIsSignIn = false;
        })
            .fail(function (e) {
                console.log("signout fail.")
                UserIsSignIn = true;
            })
            .always(function () {
                UserSignOutStatus();
            });
    }
    function UserSignOutStatus() {
        if (!UserIsSignIn) {
            $("#idSignOutButton").addClass("hidden");
            $("#idLogInButton").removeClass("hidden");
            $(".registerUser").removeClass("hidden");
            $("#userInfo").addClass("hidden");
            $(".glyphicon-duplicate").addClass("hidden");
            $(".glyphicon-hdd").addClass("hidden");
            $(".progress").addClass("hidden");
            $(".compareStorage").addClass("hidden");

            if (userPushButtonToSignOut) {
                // om användaren loggar in; ta bort tbody och ersätt den med en tom tbody.. 
                //$("tbody").replaceWith("<tbody></tbody>");
                ClearDataTable();
            }
        } else {
            $("#idSignOutButton").removeClass("hidden");
            $("#idLogInButton").addClass("hidden");
            $(".registerUser").addClass("hidden");
            $("#userInfo").removeClass("hidden");
            $(".glyphicon-duplicate").removeClass("hidden");
            $(".glyphicon-hdd").removeClass("hidden");
            $(".progress").removeClass("hidden");
            $(".compareStorage").removeClass("hidden");
        }
    }

    // knapp för att registrera sig
    $('.registerUser').click(function () {
        $("#boxRegister").removeClass("hidden");
        $('#overlay').fadeIn(200, function () {
            $('#boxErrorRegister').animate({ 'top': '-200px' }, 500);
            $('#boxRegister').animate({ 'top': '200px' }, 200);
            $("#boxErrorRegister").addClass("hidden");
        });
        return false;
    });

    // knapp för att öppna inloggning igen efter error att logga in
    $('.signinuseragain').click(function () {
        $("#box").removeClass("hidden");
        $('#overlay').fadeIn(200, function () {
            $('#boxErrorLogin').animate({ 'top': '-200px' }, 500);
            $("#boxErrorLogin").addClass("hidden");
            $('#box').animate({ 'top': '200px' }, 200);
        });
        return false;
    });

    //one x to x them all. Kryssknappen 
    $('.boxclose').click(function () {
        $('.box').animate({ 'top': '-200px' }, 500);
        $('#overlay').fadeOut('fast');
        $(".box").addClass("hidden");
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

        }).done(function () {
            console.log("ajax call - success")
            UserIsSignIn = true;
            getUserAccountInfo(); // <--------------------------------------------- hämtar userinfo
        })
  .fail(function () {
      UserIsSignIn = false;
      message = "ajax call error. ";
  })
            .always(function () {
                UserSigninStatus();
            });
    }

    function UserSigninStatus() {
        // få knapp att sluta snurra
        $btn.button('reset')

        // valideringen har gått igenom
        if (UserIsSignIn) {
            $('#box').animate({ 'top': '-200px' }, 200, function () {
                $('#overlay').fadeOut('fast');
            });
            $("#idLogInButton").addClass("hidden");
            $(".registerUser").addClass("hidden");
            $("#idSignOutButton").removeClass("hidden");
            $("#userInfo").removeClass("hidden");
            $(".glyphicon-duplicate").removeClass("hidden");
            $(".glyphicon-hdd").removeClass("hidden");
            $(".progress").removeClass("hidden");
            $("#box").addClass("hidden");
            $(".compareStorage").removeClass("hidden");
            // kalla global metod för att ladda listan
            GetFileItems();
            getUserAccountInfo();
            return false;
        }
            // nåt gick snett
        else if (!UserIsSignIn) {
            $(".result").text(message);
            $('#box').animate({ 'top': '-200px' }, 500, function () {
                $('#boxErrorLogin').animate({ 'top': '200px' }, 200);
            });
            $("#box").addClass("hidden");
            $("#boxErrorLogin").removeClass("hidden");
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

        }).done(function () {
            UserIsSignIn = true;
            userSignInAfterRegister(userName, userPassword);
        })
  .fail(function () {
      UserIsSignIn = false;
      message = "Error register. ";
      UserRegisterStatus();
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

        }).done(function () {
            UserIsSignIn = true;
            UserRegisterStatus();
        })
  .fail(function () {
      UserIsSignIn = false;
      message = "ajax call - error sign in after register. ";
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
            $("#boxRegister").addClass("hidden");
            // kalla global metod för att ladda listan
            GetFileItems();
        } else if (!UserIsSignIn) {
            //sätt felmeddelande på error popup
            $(".result").text(message);
            $('#boxRegister').animate({ 'top': '-200px' }, 200, function () {
                $('#boxErrorRegister').animate({ 'top': '200px' }, 200);
            });
            $("#boxRegister").addClass("hidden");
            $("#boxErrorRegister").removeClass("hidden");
        }
    }
    /*END JS Sign in*/
});