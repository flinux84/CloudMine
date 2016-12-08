var table;

var HTMLappender = function (element) {
    table = element;
    console.log('appending html');
    HTMLappender.prototype.appendTable = function (result) {
        var i;
        for(i = 0; i < result.length; i++){
            table.append('<tr><td>' + result[i].id + '</td><td>' + result[i].fileName + '</td><td>' + result[i].fileSize + '</td><td>' + result[i].uploaded.split('T')[0] + '</td><td>' + result[i].dataType + '</td><td>' + result[i].description + '</td></tr>');
        }         
    }
    
    HTMLappender.prototype.removefromTable = function (result) {

    }

}
