function dragAndDrop(dropzone) {

    dropzone.on('dragenter', function (e) {
        e.preventDefault();
        e.stopPropagation();
        $(this).css('border', '2px solid #0B85A1');
    });

    dropzone.on('dragover', function (e) {
        e.preventDefault();
        e.stopPropagation();
    });

    dropzone.on('drop', function (e) {

        $(this).css('border', '2px dotted #0B85A1');
        e.preventDefault();
        e.stopPropagation();

        var files = e.originalEvent.dataTransfer.files;

        dragDropUpload(files, dropzone);
    });
};

$(document).on('dragenter', function (e) {
    e.stopPropagation();
    e.preventDefault();
});
$(document).on('dragover', function (e) {
    e.stopPropagation();
    e.preventDefault();
    dropzone.css('border', '2px dotted #0B85A1');
});
$(document).on('drop', function (e) {
    e.stopPropagation();
    e.preventDefault();
});


function dragDropUpload(files, dropzone) {
    for (var i = 0; i < files.length; i++) {
        var fd = new FormData();
        fd.append('file', files[i]);    
        console.log("Uploading file " + files[i].name);
        //var status = new createStatusbar(obj); //Using this we can set progress.
        //status.setFileNameSize(files[i].name, files[i].size);

        //sendFileToServer(fd, status); anropa uploaden i Ali/Joakims kod, ändra när den är klar.
    }
}