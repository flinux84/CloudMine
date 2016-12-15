using CloudMineServer.API_server.Services;
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

        // Kolla om chunken finns redan på dataChunks genom att kolla på checksum. 
        public async Task<bool> CheckChecksum(string userId, string checksum)
        {
            var ListFileItems = await _context.FileItems.Include(fi => fi.DataChunks).Where(x => x.UserId == userId).Select(x => x.DataChunks).ToListAsync();
            var checkSums = ListFileItems.SelectMany(fi => fi).Select(dc => dc.Checksum);

            if (checkSums == null)
                return false;
            foreach (var c in checkSums)
            {
                if (c == checksum)
                    return true;
            }
            return false;
        }

        // Kolla om chunken finns redan på FileItem 
        // TODO: test
        public async Task<bool> CheckChecksumOnFileItem(string userId, string checksum)
        {
            var ListFileItems = await _context.FileItems.Where(x => x.UserId == userId).ToListAsync();
            var checkSums = ListFileItems.Any(y => y.Checksum == checksum);

            if (!checkSums)
            {
                return false;
            }
            return true;
        }

        //TODO: om fileitem redan finns, kolla first default på cunken, ta ut part name. kolla om det finns så många chunks som den säger ska finnas. retunera bool. 
        // TODO: Test
        public async Task<bool> DoesAllChunksExist(int fileItemID)
        {
            var firstDataChunk = await _context.DataChunks.FirstOrDefaultAsync(y => y.FileItemId == fileItemID);
          
            var NumberOfChunksInSequence = firstDataChunk.NumberOfChunksInSequence();

            //lista med datachunks som tillhör ett fileitem
            //var ListFileItems = await _context.FileItems.Include(t => t.DataChunks).Where(x => x.Id == fileItemID).Select(x => x.DataChunks).ToListAsync();
            //var acdc = ListFileItems.Count();

            var fi = _context.FileItems.Include(w => w.DataChunks).FirstOrDefault(z => z.Id == fileItemID);
            var actualChunksInFileItem = fi.DataChunks.Count();

            if(actualChunksInFileItem == NumberOfChunksInSequence)
            {
                // Antalet chunks stämmer med antalet som ska finnas 
                return true;
            }
            // Antalet chunks som ska finnas och antalet som faktiskt finns stämmer inte
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

            var returnUri = FM.MakeFileOnServer(fi);

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

        // Get spcific datachunk - under construction! 
        public async Task<DataChunk> GetSpecifikDataChunk(int FileItemId, int datachunkIndex)
        {
            // Finns inget "datachunkIndex" i DataChunks att söka nästa datachunk på. Men tanken är att man kanske kan genom ett 
            // FileItems id ta ut en grupp av datachunks och kunna plocka ut ett specifikt datachunk objekt. 
            // Har inte hunnit fundera på hur man ska använda sig av det än eller vilket som blir bästa tillvägagångssätt.
            // En datachunk hade behövt veta vilket index den har och vilken som nästa index för att kunna ta ut..

            //Hypotetiskt i nuvarande metod skulle (y => y.Id == datachunkIndex) bytas till nåt i stil med (y => y.datachunkIndex == datachunkIndex)
            var dc = await _context.DataChunks.Where(x => x.FileItemId == FileItemId).FirstOrDefaultAsync(y => y.Id == datachunkIndex);
            return dc;
        }

        public async Task<DataChunk> GetFirstDataChunk(int fileItemId)
        {
            var anyDataChunk = await _context.DataChunks.FirstOrDefaultAsync(d => d.FileItemId == fileItemId);
            var firstName = anyDataChunk.FirstInSequenceName();
            var firstDataChunk = await _context.DataChunks.FirstOrDefaultAsync(d => d.PartName == firstName && d.FileItemId == fileItemId);
            return firstDataChunk;
        }
        public async Task<DataChunk> GetNextDataChunk(DataChunk dataChunk)
        {
            var nextName = dataChunk.NextName();
            if (nextName == null)
                return null;
            var nextDataChunk = await _context.DataChunks.FirstOrDefaultAsync(d => d.PartName == nextName && d.FileItemId == dataChunk.FileItemId);
            return nextDataChunk;
        }
        public async Task<DataChunk> GetDataChunkAtIndex(DataChunk dataChunk, int index)
        {
            var partName = dataChunk.ChunkNameAtIndex(index);
            var dataChunkAtIndex = await _context.DataChunks.FirstOrDefaultAsync(d => d.PartName == partName && d.FileItemId == dataChunk.FileItemId);
            return dataChunkAtIndex;
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
