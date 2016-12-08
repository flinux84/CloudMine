using CloudMineServer.Data;
using CloudMineServer.Interface;
using CloudMineServer.Models;
using Microsoft.AspNetCore.Identity;
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

        private readonly CloudDbRepository _context;
        private readonly ApplicationDbContext _appDbContext;

        public CloudMineDbService(CloudDbRepository context, ApplicationDbContext appDbContext)
        {
            _context = context;
            _appDbContext = appDbContext;
        }

        #endregion

        #region CRUD

        // Användaren vill lägga till en ny fil, lägg till metadata till db. 
        public async Task<bool> InitCreateFileItem(FileItem fi)
        {
            bool checkSize = await CheckStorageSpace(fi);
            if (!checkSize)
            {
                return false;
            }

            // Skapa sträng som ska retuneras
            string GuidToString = "";

            // Skapa ett GuId
            Guid guid = Guid.NewGuid();

            // Lägg till GuId till Metadata
            // fi.Checksum = guid;

            // Lägg till datum som filen laddas upp
            fi.Uploaded = DateTime.Now;

            // Lägg till metadata till db
            bool add = await Add(fi);

            // Om db save går bra retunera GuId
            if (add)
            {
                GuidToString = guid.ToString();
                //return GuidToString;
                return true;
            }

            // Om save inte går, retunera ""
            //return GuidToString;
            return false;
        }

        // Kolla om chunken finns redan
        public async Task<bool> CheckChecksum(string userId, string checksum)
        {
            var ListFileItems = await _context.FileItems.Include(fi => fi.DataChunks).Where(x => x.UserId == userId).Select(x => x.DataChunks).ToListAsync();
            var checkSums = ListFileItems.SelectMany(fi => fi).Select(dc => dc.Checksum);

            if (checkSums == null)
                return false;
            foreach(var c in checkSums)
            {
                if (c == checksum)
                    return true;
            }
            return false;
        }

        // Create Add DataChunk
        public async Task<bool> AddFileUsingAPI(DataChunk DC)
        {
            bool add = await Add(DC);
            if (!add)
            {
                return false;
            }
            return true;

        }

        // Read (All) METADATA
        public async Task<FileItemSet> GetAllFilesUsingAPI(string userID)
        {
            var ListFileItems = await _context.FileItems.Where(x => x.UserId == userID).ToListAsync();

            FileItemSet returnFileITem = new FileItemSet() { ListFileItems = ListFileItems };

            return returnFileITem;
        }

        // Read (One) METADATA
        public async Task<FileItem> GetFileByIdUsingAPI(int id)
        {
            var fi = await _context.FileItems.FirstOrDefaultAsync(x => x.Id == id);

            return fi;
        }

        // Update METADATA
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
            var fi = await _context.FileItems.Include(x => x.DataChunks).FirstOrDefaultAsync(x => x.Id == num);
            bool check = await Delete(fi);
            return check;
        }

        #endregion

        #region FilItem & DataChunks

        // Read One with file with filechunks return URI
        public async Task<Uri> GetSpecificFilItemAndDataChunks(int id, string userId)
        {
            var IQuerybleFileItem = _context.FileItems.Include(x => x.DataChunks).Where(x => x.UserId == userId);
            var fi = await IQuerybleFileItem.FirstOrDefaultAsync(x => x.Id == id);

            Services.FileMerge FM = new Services.FileMerge();

            var returnUri = FM.MakeFile(fi);

            return returnUri;
        }


        // Read One with file with filechunks return FileItem. No Merge on Server
        public async Task<FileItem> GetSpecifikFileItemAndDataChunk(int id, string userId)
        {
            var IQuerybleFileItem = _context.FileItems.Include(x => x.DataChunks).Where(x => x.UserId == userId);
            var fi = await IQuerybleFileItem.FirstOrDefaultAsync(x => x.Id == id);

            return fi;
        }

        // Read All with file with filechunks. No Merge on Server
        public async Task<List<FileItem>> GetAllFilItemAndDataChunks(string userId)
        {
            var IQuerybleFileItem = _context.FileItems.Include(x => x.DataChunks).Where(x => x.UserId == userId);
            return await IQuerybleFileItem.ToListAsync();
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

        private async Task<bool> CheckStorageSpace(FileItem FI)
        {
            var user = await _appDbContext.Users.Where(u => u.Id == FI.UserId).SingleOrDefaultAsync();
            int storageSize = user.StorageSize;
            int check = FI.FileSize;

            // Kolla först så filen som ska laddas upp inte i sig är större än tillåtet.
            if (check > storageSize)
            {
                return false;
            }

            // Räkna ihop användarens tidigare storlek på filer
            var allUserFileItem = _context.FileItems.Where(x => x.UserId == FI.UserId);

            foreach (var FileItem in allUserFileItem)
            {
                check += FileItem.FileSize;
            }

            if (check <= storageSize)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
