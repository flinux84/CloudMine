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

        //private readonly ApplicationDbContext _identity;
        private readonly CloudDbRepository _context;

        private int AllowedStorage = 100;

        public CloudMineDbService(/*ApplicationDbContext identityContext,*/ CloudDbRepository context)
        {
            _context = context;
            //_identity = identityContext;
        }


        #endregion

        #region CRUD

        // Användaren vill lägga till en ny fil, lägg till metadata till db. 
        public async Task<string> InitCreateFileItem(FileItem fi)
        {
            // Skapa sträng som ska retuneras
            string GuidToString = "";

            // Skapa ett GuId
            Guid guid = Guid.NewGuid();

            // Lägg till GuId till Metadata
            fi.Checksum = guid;

            // Lägg till metadata till db
            bool add = await Add(fi);

            // Om db save går bra retunera GuId
            if (add)
            {
                GuidToString = guid.ToString();
                return GuidToString;
            }

            // Om save inte går, retunera ""
            return GuidToString;
        }

        // Create
        public async Task<string> AddFileUsingAPI(DataChunk DC)
        {

            // TODO: kolla size också innan add! bool checkSize = CheckStorageSpace();
            //if (!checkSize)
            //{
            //    return "Not enough storage";
            //}

            bool add = await Add(DC);
            if (!add)
            {
                return "error adding DataChunk";
            }
            return "Ok";

        }

        // Read (All) METADATA
        public async Task<FileItemSet> GetAllFilesUsingAPI(string userID)
        {
            var ListFileItems = await _context.FileItems.Where(x => x.UserId == userID).ToListAsync();

            FileItemSet returnFileITem = new FileItemSet() { ListFileItems = ListFileItems };

            return returnFileITem;
        }

        // Read (One) 
        public async Task<FileItem> GetFileByIdUsingAPI(int id)
        {
            var fi = await _context.FileItems.FirstOrDefaultAsync(x => x.Id == id);

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
        public async Task<bool> UpDateByIdUsingAPI(int num, FileItem item)
        {
            if (num == item.Id)
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

        #region Internal Helper
        // Kolla storlek på tillgängligt utrymme. Förutsatt att size tas ut på klienten. Alt skulle vara att kolla size på bit array, på serven, (*).
        //private async Task<bool> CheckStorageSpace(FileItemSet FIS)
        //{
        //    List<FileItem> chekSumFileSize = new List<FileItem>();
        //    int countSize = 0;

        //    // Kolla total storlek på fil som skickas.
        //    foreach (var file in FIS.ListFileItems)
        //    {
        //        countSize += file.FileSize;
        //        //(*) int s = file.FileData.Length;
        //    }

        //    // Hämta lista med användarens redan sparade filer.
        //    if (FIS.ListFileItems != null)
        //    {
        //        chekSumFileSize = await _context.FileItems.Where(x => x.UserId == FIS.UserId).ToListAsync();

        //        // Lägg till redan sparade filers storlek. 
        //        foreach (var item in chekSumFileSize)
        //        {
        //            countSize += item.FileSize;
        //        }
        //    }

        //    // Om totalen mindre än tillåten storlek retunera true
        //    if (countSize <= AllowedStorage)
        //    {
        //        return true;
        //    }

        //    // Annars om mer än tillåten storlek retunera false
        //    return false;
        //}
        #endregion
    }
}
