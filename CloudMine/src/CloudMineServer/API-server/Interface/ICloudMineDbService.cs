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
        Task<bool> AddFileUsingAPI(FileItemSet item);
        Task<FileItemSet> GetAllFilesUsingAPI(FileItemSet item);
        Task<FileItem> GetFileByIdUsingAPI(string num);
        Task<bool> UpDateByIdUsingAPI(string num, FileItem item);
        Task<bool> DeleteByIdUsingAPI(string num);
        #endregion
    }
}
