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
        Task<FileItem> GetFileByIdUsingAPI(int num);
        Task<bool> UpDateByIdUsingAPI(int num, FileItem item);
        Task<bool> DeleteByIdUsingAPI(int num);
        #endregion
    }
}
