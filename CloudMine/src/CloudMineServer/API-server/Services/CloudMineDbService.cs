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
    public class CloudMineDbService : ICloudMineDbService
    {

        #region Dependency Injection Constructor

        private readonly ApplicationDbContext _identity;
        private readonly CloudDbRepository _context;

        private int AllowedStorage = 100;

        public CloudMineDbService(ApplicationDbContext identityContext, CloudDbRepository context)
        {
            _context = context;
            _identity = identityContext;
        }


        #endregion

        #region CRUD

        // Create
        public async Task<bool> AddFileUsingAPI(FileItemSet FIS)
        {
            bool add = true;
            // TODO: kolla size också innan add! add = CheckStorageSpace();

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
            if (item.ListFileItems == null) //Listan borde vara null 
            {
                item.ListFileItems = await _context.FileItems.Where(x => x.UserId == item.UserId).ToListAsync();
            }

            return item;
        }

        // Read (One) - about to be deprecated
        public async Task<FileItem> GetFileByIdUsingAPI(string id)
        {
            var fi = await _context.FileItems.FirstOrDefaultAsync(x => x.Id.ToString() == id);

            return fi;
        }

        // Read (One) Return FileItemSet with all chunks. TODO: Testa att den fungerar!
        //public async Task<FileItemSet> GetFileChunsByIdAndUserId(string Id, string UserId)
        //{
        //    // Ta ut användarens alla filer
        //    var ListUserFiles = await _context.FileItems.Where(x => x.UserId == UserId).ToListAsync();

        //    // Hitta objekt som stämmer med id (id på det objektet som sak hämtas)
        //    var fi = ListUserFiles.FirstOrDefault(x => x.Id == Id);

        //    // En fil kan vara uppdelad i flera chunks med olika id, fast med samma FileChunkId. Ta ut fileChunkId
        //    int fcId = fi;
        //    var ListOfFileItem = ListUserFiles.Where(x => x.FileChunkId == fcId).ToList();

        //    // Skapa FileItemSet objekt. Lägg till lista av chunks till som filer i FileItemSet
        //    FileItemSet returnObj = new FileItemSet() { UserId = UserId, ListFileItems = ListOfFileItem };

        //    // Retunera skapat objekt
        //    return returnObj;
        //}

        // Update
        public async Task<bool> UpDateByIdUsingAPI(string num, FileItem item)
        {
            if (num == item.Id.ToString())
            {
                bool check = await Update(item);
                return check;
            }
            return false;
        }

        // Delete
        public async Task<bool> DeleteByIdUsingAPI(string num)
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

        #region Internal Helper
        // Kolla storlek på tillgängligt utrymme. Förutsatt att size tas ut på klienten. Alt skulle vara att kolla size på bit array, på serven, (*).
        private async Task<bool> CheckStorageSpace(FileItemSet FIS)
        {
            List<FileItem> chekSumFileSize = new List<FileItem>();
            int countSize = 0;

            // Kolla total storlek på fil som skickas.
            foreach (var file in FIS.ListFileItems)
            {
                countSize += file.FileSize;
                //(*) int s = file.FileData.Length;
            }

            // Hämta lista med användarens redan sparade filer.
            if (FIS.ListFileItems != null)
            {
                chekSumFileSize = await _context.FileItems.Where(x => x.UserId == FIS.UserId).ToListAsync();

                // Lägg till redan sparade filers storlek. 
                foreach (var item in chekSumFileSize)
                {
                    countSize += item.FileSize;
                }
            }

            // Om totalen mindre än tillåten storlek retunera true
            if (countSize <= AllowedStorage)
            {
                return true;
            }

            // Annars om mer än tillåten storlek retunera false
            return false;
        }
        #endregion
    }
}
