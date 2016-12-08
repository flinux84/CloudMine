var width = 0;

var ProgressBar = function (divDOM, barDOM, fileDOM) {

    divDOM.css('display', 'none');
    barDOM.innerHTML = '';
    console.log("progressbar loaded");

    ProgressBar.prototype.updateProgress = function (percent, filename) {

        console.log("uploading file" + filename);
        divDOM.css('display', 'block');
        fileDOM.innerHTML = filename;

        if (width >= 100) {
            setTimeout(clear, 2000)
            console.log("finished upload of " + filename);
        } else {
            width = percent;
            barDOM.css('width', width + '%');
            barDOM.innerHTML = width + '%';
        }     
    }

    ProgressBar.prototype.DoATestRun = function () {
        divDOM.css('display', 'block');
        fileDOM.append("Uploading filename.png");
        var id = setInterval(frame, 100);
        console.log("doing a testrun");
        function frame() {
            barDOM.css('width', width + '%');
            width++;
            if (width >= 100) {
                clearInterval(id);
                setTimeout(clear, 2000)
            }
        }
    }


    function clear() {
        width = 0;
        divDOM.css('display', 'none');
    }  

}






