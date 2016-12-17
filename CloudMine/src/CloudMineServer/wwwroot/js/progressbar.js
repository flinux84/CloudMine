var width = 0;

var ProgressBar = function (divDOM, barDOM, fileDOM) {

    divDOM.css('display', 'none');
    barDOM.text('Loading');
    console.log("progressbar loaded");

    ProgressBar.prototype.updateProgress = function (percent, filename) {

        console.log("loading file" + filename);
        divDOM.css('display', 'block');
        fileDOM.text(filename);
        width = percent;

        if (width >= 100) {
            barDOM.css('width', width + '%');
            barDOM.text(width + '%');
            console.log("finished upload of " + filename);
            setTimeout(clear, 1000)
        } else {
            barDOM.css('width', width + '%');
            barDOM.text(width + '%');
        }     
    }

    ProgressBar.prototype.clearBar = clear();

function clear() {
        width = 0;
        divDOM.css('display', 'none');
        barDOM.text('');
        fileDOM.text('');
    }  

}

function ShowLoading() {
    $('#loading').fadeIn();
}
function HideLoading() {
    $('#loading').fadeOut();
}





