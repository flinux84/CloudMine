var table;
var downloadbutton = '<span class=\"glyphicon glyphicon-save\"></span>';
var grey = { "color": "grey" };
var disabled = { "display": "none" };

var HTMLappender = function (element) {
    table = element;
    console.log('appending html');
    HTMLappender.prototype.appendTable = function (result) {

        var bool = Object.prototype.toString.call(result) === "[object Array]";

        if (bool === true) {
            var i;
            for (i = 0; i < result.length; i++) {
                table.append('<tr id=' + 'r' + result[i].id + '><td>' + result[i].fileName
                + '</td><td>' + result[i].fileSize
                + '</td><td>' + result[i].uploaded.split('T')[0]
                + '</td><td>' + result[i].dataType
                + '</td><td>' + result[i].description
                + '</td><td><a href=\"/api/v1.0/GetFile/NoDisk/' + result[i].id + '\">'
                + downloadbutton + '</a><a href="#" class="glyphicon glyphicon-pencil edit-button"></a> '
                + '<span class=\"glyphicon glyphicon-remove-sign\" style="cursor: pointer" ' + 'id=' + result[i].id + '" '
                + 'onClick="DeleteFileItem('
                + result[i].id + ')">' + '</span>' + '</td></tr>');
                if (result[i].isComplete === false) {
                    $('#' + 'r' + result[i].id).css(grey);
                    $('#' + 'r' + result[i].id).children().children('a').css(disabled);
                }
            }
        }
        else {
            table.append(standardRow(result));
        }

        //adding click events to edit buttons after they are created
        $('.edit-button').click(function (e) {
            e.PreventDefault;
            var myId = $(this).parent().parent()[0].id;
            //remove 'r' from id
            myId = myId.slice(1, myId.length);
            BuildEditForm(myId);
        })
    }

    HTMLappender.prototype.deleteRow = function (fileitemId) {
        $('#' + 'r' + fileitemId).remove();
    }

    HTMLappender.prototype.addOrReplaceRow = function (result) {

        if ($('#' + 'r' + result.id).length > 0) {
            $('#' + 'r' + result.id).replaceWith(standardRow(result))
        }
        else {
            table.append(standardRow(result));
        }

        if (result.isComplete === false) {
            $('#' + 'r' + result.id).css(grey);
            $('#' + 'r' + result.id).children().children('a').css(disabled);
        }

    }

    function standardRow(result) {
        var tablerow = '<tr id=' + 'r' + result.id + '><td>' + result.fileName
        + '</td><td>' + result.fileSize
        + '</td><td>' + result.uploaded.split('T')[0]
        + '</td><td>' + result.dataType
        + '</td><td>' + result.description
        + '</td><td><a href=\"/api/v1.0/GetFile/NoDisk/' + result.id + '\">'
        + downloadbutton + '</a><a href="#" class="glyphicon glyphicon-pencil edit-button"></a> '
        + '<span class=\"glyphicon glyphicon-remove-sign\" style="cursor: pointer" ' + 'id=' + result.id + '" '
        + 'onClick="DeleteFileItem('
        + result.id + ')">' + '</span>' + '</td></tr>'
        return tablerow;
    }

}

function UserAccountInfo() {
    $.ajax({
        type: "POST",
        url: '../api/v1.0/FileItems/' + FileID,
        contentType: false,
        processData: false,
        data: FD,
        success: function (result) {

        }

    });
};

function BuildEditForm(id) {
    $.getJSON('/api/v1.0/FileItems/' + id).done(function (response) {
        editFileItem = response;
        var editForm = $('#edit-form');
        //Vi kanske inte vill ändra filename
        editForm.append('<div class="form-group">' +
                            '<label for="edit-filename">Filename</label>' +
                            '<input type="text" class="form-control" id="edit-filename" placeholder="Filename" value="' + response['fileName'] + '">' +
                        '</div>');
        editForm.append('<div class="form-group">' +
                            '<label for="edit-description">Description</label>' +
                            '<textarea type="text" class="form-control" id="edit-description" placeholder="Description">' + response['description'] + '</textarea>' +
                        '</div>');
        editForm.append('<input type="hidden" id="edit-id" value="' + response['id'] + '">');
        $('#edit-dialog').dialog("open");
    }
    );
}
