var table;
var downloadbutton = '<span class=\"glyphicon glyphicon-save\"></span>';
//var deletebutton = '<span class=\"glyphicon glyphicon-remove-sign\" onClick=\"DeleteFileItem()"></span>';

var HTMLappender = function (element) {
    table = element;
    
    HTMLappender.prototype.appendTable = function (result) {
        console.log('appending html');
        var i;
        if (typeof result === 'object' || result != undefined) {
            table.append('<tr><td>' + result.fileName
                + '</td><td>' + result.fileSize
                + '</td><td>' + result.uploaded
                + '</td><td>' + result.dataType
                + '</td><td>' + result.description
                + '</td><td><a href=\"/api/v1.0/GetFile/NoDisk/' + result.id + '\">'
                + downloadbutton + '</a>'
                + '<span class=\"glyphicon glyphicon-remove-sign\"' + 'onClick=DeleteFileItem('
                + result.id + ')>' + '</span>' + '</td></tr>');

        }
        else{
            for (i = 0; i < result.length; i++) {
                table.append('<tr><td>' + result[i].fileName
                    + '</td><td>' + result[i].fileSize
                    + '</td><td>' + result[i].uploaded.split('T')[0]
                    + '</td><td>' + result[i].dataType
                    + '</td><td>' + result[i].description
                    + '</td><td><a href=\"/api/v1.0/GetFile/NoDisk/' + result[i].id + '\">'
                    + downloadbutton + '</a>'
                    + '<span class=\"glyphicon glyphicon-remove-sign\"' + 'onClick=DeleteFileItem('
                    + result[i].id + ')>' + '</span>' + '</td></tr>');
            }
        }         
    }
    
}
