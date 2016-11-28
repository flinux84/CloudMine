// Write your Javascript code.
function UploadFile(TargetFile) {

    var FileChunk = [];
    var file = TargetFile[0];
    var MaxFileSizeMB = 1;
    var BufferChunkSize = MaxFileSizeMB * (1024 * 1024);
    var ReadBuffer_Size = 1024;
    var FileStreamPos = 0;
    var EndPos = BufferChunkSize;
    var Size = file.size;

    while (FileStreamPos < Size) {
        FileChunk.push(file.slice(FileStreamPos, EndPos));
        FileStreamPos = EndPos; // jump by the amount read
        EndPos = FileStreamPos + BufferChunkSize; // set next chunk length
    }

    var TotalParts = FileChunk.length;
    var PartCount = 0;

    while (chunk = FileChunk.shift()) {
        PartCount++;
        var FilePartName = file.name + ".part_" + PartCount + "." + TotalParts;
        UploadFileChunk(chunk, FilePartName);
    }
}