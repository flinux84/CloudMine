﻿var table;
var downloadbutton = '<span class=\"glyphicon glyphicon-save\"></span>';
//var deletebutton = '<span class=\"glyphicon glyphicon-remove-sign\" onClick=\"DeleteFileItem()"></span>';

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
}

function UserAccountInfo() {
    $.ajax({
        type: "POST",
        url: '../api/v1.0/Users/UserInfo',
        contentType: false,
        processData: false,
        data: FD,
        success: function (result) {

        }

    });
    };
    
