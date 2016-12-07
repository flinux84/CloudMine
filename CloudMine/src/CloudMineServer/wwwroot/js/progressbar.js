function createProgressbar(progressDiv, progressBar, fileLabel) {
    var div = progressDiv;
    var elem = progressBar;
    var label = fileLabel;
    var width = 0;
    elem.innerHTML = '';
    console.log("progressbar loaded");

    this.updateProgress = function (percent, filename) {
        var perc = percent;
        var file = filename;
        console.log(percent + "percent loaded of file" + filename);
        div.style.display = "block";
        label.innerHTML = file;
        var id = setInterval(frame, 1000);

        if (width >= 100) {
            clearInterval(id);
            setTimeout(clear, 1000)
            div.style.display = "hidden";
        } else {
            width = perc;
            elem.style.width = percent + '%';
            elem.innerHTML = width + '%';
        }     
    }

    function frame() {
        elem.style.width = width + '%';
        console.log("updating progressbar...");
    }

    this.finished = function clear() {
        width = 0;
        elem.style.display = "hidden";
        console.log("clears progress, done");
    }

}






