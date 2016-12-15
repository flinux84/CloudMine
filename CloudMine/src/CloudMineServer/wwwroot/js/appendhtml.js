var table;
var downloadbutton = '<span class=\"glyphicon glyphicon-save\"></span>';
var grey = {"color":"grey","pointer-events":"none","cursor":"default"};

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
                + downloadbutton + '</a>'
                + '<span class=\"glyphicon glyphicon-remove-sign\" style="cursor: pointer" ' + 'id=' + result[i].id + '" '
                + 'onClick="DeleteFileItem('
                + result[i].id + ')">' + '</span>' + '</td></tr>');
                if (result[i].isComplete === false) {
                    $('#' + 'r' + result.id).css(grey);
                }
            }
        }

        else {
            table.append(standardRow(result));
        }
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
            if (result.isComplete === false) {
                $('#' + 'r' + result.id).css(grey);
            }
        }

        if (result.isComplete === false) {            
            $('#' + 'r' + result.id).css(grey);
        }
       
    }
    
    function standardRow(result) {
        var tablerow = '<tr id=' + 'r' + result.id + '><td>' + result.fileName
        + '</td><td>' + result.fileSize
        + '</td><td>' + result.uploaded.split('T')[0]
        + '</td><td>' + result.dataType
        + '</td><td>' + result.description
        + '</td><td><a href=\"/api/v1.0/GetFile/NoDisk/' + result.id + '\">'
        + downloadbutton + '</a>'
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
    
