var table;
var downloadbutton = '<span class=\"glyphicon glyphicon-save\"></span>';


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
                + '<span class=\"glyphicon glyphicon-remove-sign\"' + 'id=' + result[i].id + '" '
                + 'onClick="DeleteFileItem('
                + result[i].id + ')">' + '</span>' + '</td></tr>');
            }
        }

        else {
            table.append('<tr id=' + 'r' + result.id + '><td>' + result.fileName
        + '</td><td>' + result.fileSize
        + '</td><td>' + result.uploaded.split('T')[0]
        + '</td><td>' + result.dataType
        + '</td><td>' + result.description
        + '</td><td><a href=\"/api/v1.0/GetFile/NoDisk/' + result.id + '\">'
        + downloadbutton + '</a>'
        + '<span class=\"glyphicon glyphicon-remove-sign\"' + 'id=' + result.id + '" '
        + 'onClick="DeleteFileItem('
        + result.id + ')">' + '</span>' + '</td></tr>');
        }
    }

    HTMLappender.prototype.deleteRow = function (fileitemId) {
        $('#' + 'r' + fileitemId).remove();
    }

    HTMLappender.prototype.replaceRow = function (result) {
        $('#' + 'r' + result.id).replaceWith(standardRow(result))
    }
    
    function standardRow(result) {
        var table = '<tr id=' + 'r' + result.id + '><td>' + result.fileName
        + '</td><td>' + result.fileSize
        + '</td><td>' + result.uploaded.split('T')[0]
        + '</td><td>' + result.dataType
        + '</td><td>' + result.description
        + '</td><td><a href=\"/api/v1.0/GetFile/NoDisk/' + result.id + '\">'
        + downloadbutton + '</a>'
        + '<span class=\"glyphicon glyphicon-remove-sign\"' + 'id=' + result.id + '" '
        + 'onClick="DeleteFileItem('
        + result.id + ')">' + '</span>' + '</td></tr>'
        return table;
    }

    HTMLappender.prototype.userAccountInfo = function (result) {
        
        var theStorageSize = Math.round((result.storageSize)/1000000);
        var theStorageUsed =((result.usedStorage) / 1000000).toFixed(1);

        var TotalStorage = Math.round(theStorageSize / theStorageUsed);


        $('#accInfo').text(result.userName);        
        $('#spaceRemaining').attr('style', 'width:'+ TotalStorage +'%');
        $('.progress-value').text(TotalStorage + '%');
        $('#storageData').text(theStorageUsed + ' Mb of ' + theStorageSize + ' Mb ');
        $('#numberOfFiles').text(" "+result.numberFiles +" "+ 'files');
    }

    

}

