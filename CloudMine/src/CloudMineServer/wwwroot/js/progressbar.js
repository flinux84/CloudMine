
var ProgressBar = function (index) {
    
    var divId = 'progressDiv' + index;
    var barId = 'progressBar' + index;
    var fileId = 'filelabel' + index;

    var generateprogress = '<div class="hidden" id="' + divId
    + '"><div class="progress-bar progress-bar-striped active" role="progressbar" id="' + barId
    + '"></div><div id="' + fileId
    + '"></div></div>';

    $("body").append(generateprogress);

    var addToBottom = 60 * index;
    var css = {"position":"fixed","width":"250px","height":"60px","margin-top":"10px","right":"30px","bottom":"30px","text-align":"center","border":"1px solid black","background-color":"white","z-index":"10"};
    $('#' + divId).css(css);
    $('#' + fileId).css({ "display": "block", "clear": "both" });
    var push = { "bottom": 30 + addToBottom + "px" };
    $('#' + divId).css(push);
    console.log("progressbar(s) loaded");

    ProgressBar.prototype.updateProgress = function (percent, filename, index) {
        $('#progressDiv' + index).removeClass('hidden');
        console.log("loading file" + filename);
        $('#filelabel' + index).text(filename);

        if (percent >= 100) {
            $('#progressBar' + index).css('width', percent + '%');
            $('#progressBar' + index).text(percent + '%');
            console.log("finished upload of " + filename);
            setTimeout(clear, 2000)
            function clear() {
                $('#progressDiv' + index).addClass('hidden');
            }
        } else {
            $('#progressBar' + index).css('width', percent + '%');
            $('#progressBar' + index).text(percent + '%');
        }     
    }
}

function ShowLoading() {
    $('#loading').fadeIn();
}
function HideLoading() {
    $('#loading').fadeOut();
}





