using CloudMineServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudMineServer.Interface
{
    public interface ICloudMineDbService
    {
        #region API CRUD
        Task<bool> InitCreateFileItem(FileItem fi);
        Task<bool> AddFileUsingAPI(DataChunk item);
        Task<FileItemSet> GetAllFilesUsingAPI(string item);
        Task<FileItem> GetFileByIdUsingAPI(int num);
        Task<bool> UpDateByIdUsingAPI(int num, FileItem item);
        Task<bool> DeleteByIdUsingAPI(int num);
        Task<bool> CheckChecksum( string userId, string checksum );
        Task<bool> CheckChecksumOnFileItem(string userId, string checksum);
        Task<bool> DoesAllChunksExist(int fileItemID);
        #endregion

        #region get FileItem & Chunks
        Task<Uri> GetSpecificFilItemAndDataChunks(int id, string userId);
        Task<List<FileItem>> GetAllFilItemAndDataChunks(string userId);
        Task<FileItem> GetSpecifikFileItemAndDataChunk(int id, string userId);
        Task<DataChunk> GetSpecifikDataChunk(int FileItemId, int datachunkIndex);
        Task<DataChunk> GetFirstDataChunk(int fileItemId);
        Task<DataChunk> GetNextDataChunk(DataChunk dataChunk);
        Task<DataChunk> GetDataChunkAtIndex(DataChunk dataChunk, int index);
        #endregion

    }
}
