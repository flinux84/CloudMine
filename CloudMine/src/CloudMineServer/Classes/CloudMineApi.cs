using CloudMineServer.Data;
using CloudMineServer.Interface;
using CloudMineServer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudMineServer.Classes
{
    public class CloudMineApi : ICloudMineApi
    {
        #region Dependency Injection Constructor

        private readonly ApplicationDbContext _context;

        public CloudMineApi(ApplicationDbContext context)
        {
            _context = context;
        }


        #endregion

        #region CRUD

        // Create
        public async Task<bool> AddFileUsingAPI(FileItemSet FIS)
        {
            bool add = true;
            // TODO: kolla size också innan add!

            foreach (var file in FIS.ListFileItems)
            {
                add = await Add(file);
                if (!add)
                {
                    return add;
                }
            }

            return add;
        }

        // Read (All)
        public async Task<FileItemSet> GetAllFilesUsingAPI(FileItemSet item)
        {
            if(item.ListFileItems == null) //Listan borde vara null 
            {
                item.ListFileItems = await _context.dbFileItem.Where(x => x.UserId == item.UserId).ToListAsync();
            }

            return item;
        }

        // Read (One)
        public async Task<FileItem> GetFileByIdUsingAPI(int num)
        {
            var fi = await _context.dbFileItem.FirstOrDefaultAsync(x => x.Id == num);

            return fi;
        }

        // Update
        public async Task<bool> UpDateByIdUsingAPI(int num, FileItem item)
        {
            if(num == item.Id)
            {
                bool check = await Update(item);
                return check;
            }
            return false;
        }

        // Delete
        public async Task<bool> DeleteByIdUsingAPI(int num)
        {
            FileItem fi = await GetFileByIdUsingAPI(num);
            bool check = await Delete(fi);
            return check;
        }

        #endregion

        #region Add, Update, Delete

        private async Task<bool> Add(object create)
        {
            try
            {
                _context.Add(create);
                await _context.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }

        private async Task<bool> Update(object edit)
        {
            try
            {
                _context.Update(edit);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            { return false; }
        }

        private async Task<bool> Delete(object delete)
        {
            try
            {
                _context.Remove(delete);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            { return false; }
        }
        #endregion

    }
}
