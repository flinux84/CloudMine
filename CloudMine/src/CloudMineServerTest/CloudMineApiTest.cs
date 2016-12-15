using CloudMineServer.Data;
using CloudMineServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

//namespace CloudMineServerTest
namespace CloudMineServer.Classes
{
    public class CloudMineApiTest
    {

        //Namnbyte: från CloudMineApi till CloudMineDbService

        #region Create Test DataBase & Arrange

        private static DbContextOptions<CloudDbRepository> CreateNewContextOptions()
        {
            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<CloudDbRepository>();
            builder.UseInMemoryDatabase()
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }

        private static DbContextOptions<ApplicationDbContext> CreateNewApplicationDbContextOptions()
        {
            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase()
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }

        private void FillTheTempDataBase(DbContextOptions<CloudDbRepository> options)
        {
            using (var context = new CloudDbRepository(options))
            {
                context.FileItems.Add(new Models.FileItem { Id = 11, UserId = "User-1a-guid-tostring", Checksum = "aaa111checksum", DataType = "typ", FileName = "name", IsComplete = true, Description = "about", FileSize = 2, Uploaded = new DateTime(2016, 12, 02) });
                context.FileItems.Add(new Models.FileItem { Id = 22, UserId = "User-2a-guid-tostring", Checksum = "bbb222checksum", DataType = "typ", FileName = "name", IsComplete = true, Description = "about", FileSize = 1, Uploaded = new DateTime(2016, 12, 02) });
                context.FileItems.Add(new Models.FileItem { Id = 33, UserId = "User-3a-guid-tostring", Checksum = "ccc333checksum", DataType = "typ", FileName = "name", IsComplete = true, Description = "about", FileSize = 1, Uploaded = new DateTime(2016, 12, 02) });
                context.FileItems.Add(new Models.FileItem { Id = 44, UserId = "User-4a-guid-tostring", Checksum = "ddd444checksum", DataType = "typ", FileName = "name", IsComplete = true, Description = "about", FileSize = 1, Uploaded = new DateTime(2016, 12, 02) });
                context.SaveChanges();
            }
        }

        private void AddInitFileItemToDb(DbContextOptions<CloudDbRepository> options)
        {
            using (var context = new CloudDbRepository(options))
            {
                Guid FileItemGuid = new Guid("976cf2f2-c675-4e27-ac7a-9f8e43f64334");
                string userGuid = "111cf2f2-c675-4e27-ac7a-9f8e43f64334";
                context.FileItems.Add(new FileItem { Id = 1, UserId = userGuid, DataChunks = null, IsComplete = false, FileSize = 111, FileName = "TEST", Description = "test", DataType = "jpg" });
                context.SaveChanges();
            }
        }

        private void AddDataChunksToDB(DbContextOptions<CloudDbRepository> options)
        {
            using (var context = new CloudDbRepository(options))
            {
                context.DataChunks.Add(new DataChunk { Id = 11, FileItemId = 1, Checksum = "aa11-checksum-fil", Data = new byte[10], PartName = "name.png.part_1.2" });
                context.DataChunks.Add(new DataChunk { Id = 22, FileItemId = 1, Checksum = "bb122-checksum-fil", Data = new byte[01], PartName = "name.png.part_2.2" });

                context.SaveChanges();
            }
        }

        private void AddDataChunksToExistingFileItemToDB(DbContextOptions<CloudDbRepository> options)
        {
            using (var context = new CloudDbRepository(options))
            {
                context.DataChunks.Add(new DataChunk { Id = 11, FileItemId = 11, Checksum = "aa11-checksum-fil", Data = new byte[10], PartName = "name.png.part_1.2" });
                context.DataChunks.Add(new DataChunk { Id = 22, FileItemId = 11, Checksum = "bb122-checksum-fil", Data = new byte[01], PartName = "name.png.part_2.2" });

                context.SaveChanges();
            }
        }

        private void AddCorruptDataChunksToExistingFileItemToDB(DbContextOptions<CloudDbRepository> options)
        {
            using (var context = new CloudDbRepository(options))
            {
                context.DataChunks.Add(new DataChunk { Id = 11, FileItemId = 11, Checksum = "aa11-checksum-fil", Data = new byte[10], PartName = "name.png.part_1.3" });
                context.DataChunks.Add(new DataChunk { Id = 22, FileItemId = 11, Checksum = "bb122-checksum-fil", Data = new byte[01], PartName = "name.png.part_2.3" });

                context.SaveChanges();
            }
        }

        private void AddUserToDB(DbContextOptions<ApplicationDbContext> options)
        {
            using (var context = new ApplicationDbContext(options))
            {
                context.Users.Add(new ApplicationUser { Id = "User-1a-guid-tostring", UserName = "TestUser", StorageSize = 100000000 });
                context.SaveChanges();
            }
        }

        #endregion

        #region Testarea

        [Fact]
        // Lägga till ett första fileitem, som ska returnera true. Gränsvärde för  user.StorageSize = 100000000;
        public async Task InitCreateFileItem_add_initial_FileItem_to_db()
        {
            //Arrange
            var options = CreateNewContextOptions();
            var appDbOptions = CreateNewApplicationDbContextOptions();
            AddUserToDB(appDbOptions);

            var fis = new FileItem() { UserId = "User-1a-guid-tostring", IsComplete = true, FileSize = 99999999, FileName = "TEST", Description = "test", DataType = "jpg" };

            using (var appDbContext = new ApplicationDbContext(appDbOptions))
            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context, appDbContext);

                //Act  
                var result = await service.InitCreateFileItem(fis);

                //Assert
                var viewResult = Assert.IsType<bool>(result);
                Assert.True(viewResult);
                Assert.Equal(1, context.FileItems.Count());
            }
        }

        [Fact]
        // Lägga till ett första fileitem med för stor fil som ska ge tillbaka false. Gränsvärde för  user.StorageSize = 100000000;
        public async Task InitCreateFileItem_to_big_file()
        {
            //Arrange
            var options = CreateNewContextOptions();
            var appDbOptions = CreateNewApplicationDbContextOptions();
            AddUserToDB(appDbOptions);

            var fis = new FileItem() { UserId = "User-1a-guid-tostring", IsComplete = true, FileSize = 100000001, FileName = "TEST", Description = "test", DataType = "jpg" };

            using (var appDbContext = new ApplicationDbContext(appDbOptions))
            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context, appDbContext);

                //Act  
                var result = await service.InitCreateFileItem(fis);

                //Assert
                var viewResult = Assert.IsType<bool>(result);
                Assert.Equal(0, context.FileItems.Count());
                Assert.False(viewResult);
            }
        }

        [Fact]
        // Användaren har redan laggt till file (size 2), men överstiger nu gränsen (+99999999). Gränsvärde för  user.StorageSize = 100000000;
        public async Task InitCreateFileItem_second_file_to_big()
        {
            //Arrange
            var options = CreateNewContextOptions();
            var appDbOptions = CreateNewApplicationDbContextOptions();
            AddUserToDB(appDbOptions);
            FillTheTempDataBase(options);

            var fis = new FileItem() { UserId = "User-1a-guid-tostring", IsComplete = true, FileSize = 99999999, FileName = "TEST", Description = "test", DataType = "jpg" };

            using (var appDbContext = new ApplicationDbContext(appDbOptions))
            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context, appDbContext);

                //Act  
                var result = await service.InitCreateFileItem(fis);

                //Assert
                var viewResult = Assert.IsType<bool>(result);
                Assert.Equal(4, context.FileItems.Count());
                Assert.False(viewResult);
            }
        }

        // Create. Lägga till datachunk
        [Fact]
        public async Task AddFileUsingAPI_add_file_and_get_true_sucess_save_to_db()
        {
            //Arrange
            var options = CreateNewContextOptions();
            var appDbOptions = CreateNewApplicationDbContextOptions();

            AddInitFileItemToDb(options);
            Guid myGuid = new Guid("976cf2f2-c675-4e27-ac7a-9f8e43f64334");

            var ds = new DataChunk() { Id = 0, FileItemId = 1, Data = new byte[10], PartName = "name.png.part_1.1" };

            using (var appDbContext = new ApplicationDbContext(appDbOptions))
            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context, appDbContext);

                //Act  
                var result = await service.AddFileUsingAPI(ds);

                //Assert
                var viewResult = Assert.IsType<bool>(result);
                Assert.True(viewResult);
                Assert.Equal(1, context.DataChunks.FirstOrDefault().FileItemId);
                Assert.Equal(1, context.FileItems.Count());
            }
        }

        // Create. försöka lägga till Datachunk, fast misslyckas med save.
        [Fact]
        public async Task AddFileUsingAPI_add_file_and_get_false_catch_save_to_db()
        {
            //Arrange
            var appDbOptions = CreateNewApplicationDbContextOptions();
            var options = CreateNewContextOptions();
            FillTheTempDataBase(options);
            AddDataChunksToDB(options);

            // Redan existerande DataChunk
            var badDC = new DataChunk() { Id = 11, FileItemId = 1, Data = new byte[10], PartName = "AudioInterface.png.part_1.2" };

            using (var appDbContext = new ApplicationDbContext(appDbOptions))
            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context, appDbContext);

                //Act  
                var result = await service.AddFileUsingAPI(badDC);

                //Assert
                var viewResult = Assert.IsType<bool>(result);
                Assert.False(result);
            }
        }

        // test om datachunken som sparas är den sista.
        [Fact]
        public async Task AddFileUsingAPI_chunk_to_add_is_last__then_change_FiliItem_bool_prop_to_true()
        {
            //Arrange
            var appDbOptions = CreateNewApplicationDbContextOptions();
            var options = CreateNewContextOptions();
            AddInitFileItemToDb(options);
            var DC = new DataChunk() { Id = 11,Checksum="", FileItemId = 1, Data = new byte[10], PartName = "AudioInterface.png.part_1.1" };

            using (var appDbContext = new ApplicationDbContext(appDbOptions))
            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context, appDbContext);

                //Act  
                var result = await service.AddFileUsingAPI(DC);

                //Assert
                Assert.True(context.FileItems.FirstOrDefault(x => x.Id == 1).IsComplete);
                var viewResult = Assert.IsType<bool>(result);
                Assert.True(result);
            }
        }

        // Read (All). Hämta användarens alla FileItems
        [Fact]
        public async Task GetAllFilesUsingAPI_get_all_the_users_files_get_a_FileItemSet()
        {
            //Arrange
            var options = CreateNewContextOptions();
            var appDbOptions = CreateNewApplicationDbContextOptions();
            FillTheTempDataBase(options);
            string userGuid = "User-1a-guid-tostring";

            using (var appDbContext = new ApplicationDbContext(appDbOptions))
            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context, appDbContext);

                //Act  
                var result = await service.GetAllFilesUsingAPI(userGuid);

                //Assert
                Assert.Equal(4, context.FileItems.Count());
                var viewResult = Assert.IsType<FileItemSet>(result);
                Assert.Equal(1, viewResult.ListFileItems.Count());
                Assert.Equal(11, viewResult.ListFileItems.FirstOrDefault().Id);
            }
        }

        // Read (One). Hämta användarens specifika FileItem/ metadata.
        [Fact]
        public async Task GetFileByIdUsingAPI_send_int_id_get_FileItem_back()
        {
            //Arrange
            var options = CreateNewContextOptions();
            var appDbOptions = CreateNewApplicationDbContextOptions();
            FillTheTempDataBase(options);
            int FileItemId = 11;

            using (var appDbContext = new ApplicationDbContext(appDbOptions))
            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context, appDbContext);

                //Act  
                var result = await service.GetFileByIdUsingAPI(FileItemId);

                //Assert
                Assert.Equal(4, context.FileItems.Count());
                var viewResult = Assert.IsType<FileItem>(result);
                Assert.Equal("User-1a-guid-tostring", viewResult.UserId);
            }
        }

        // Update. Uppdatera specifikt FileItem. 
        [Fact]
        public async Task UpDateByIdUsingAPI_send_int_id_and_FileItem_maching_id_return_true()
        {
            //Arrange
            var options = CreateNewContextOptions();
            var appDbOptions = CreateNewApplicationDbContextOptions();
            FillTheTempDataBase(options);
            var myFileItem = new FileItem() { Id = 11, IsComplete = false, FileSize = 111, FileName = "EDIT", Description = "edit", DataType = "jpg" };
            int FileItemId = 11;

            using (var appDbContext = new ApplicationDbContext(appDbOptions))
            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context, appDbContext);

                //Act  
                var result = await service.UpDateByIdUsingAPI(FileItemId, myFileItem);

                //Assert
                Assert.Equal(4, context.FileItems.Count());
                var viewResult = Assert.IsType<bool>(result);
                Assert.True(result);
                Assert.Equal("EDIT", context.FileItems.FirstOrDefault(x => x.Id == FileItemId).FileName);
            }
        }

        // Update. Int id och FileItem id stämmer inte
        [Fact]
        public async Task UpDateByIdUsingAPI_send_int_id_and_FileItem_that_dont_maching_id_return_false()
        {
            //Arrange
            var options = CreateNewContextOptions();
            var appDbOptions = CreateNewApplicationDbContextOptions();
            FillTheTempDataBase(options);
            var myFileItem = new FileItem() { Id = 11, IsComplete = false, FileSize = 111, FileName = "EDIT", Description = "edit", DataType = "jpg" };
            int FileItemId = 12345;

            using (var appDbContext = new ApplicationDbContext(appDbOptions))
            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context, appDbContext);

                //Act  
                var result = await service.UpDateByIdUsingAPI(FileItemId, myFileItem);

                //Assert
                Assert.Equal(4, context.FileItems.Count());
                var viewResult = Assert.IsType<bool>(result);
                Assert.False(viewResult);
                Assert.Equal("name", context.FileItems.FirstOrDefault(x => x.Id == 11).FileName);
            }
        }

        //TODO: test för att ändra fi som har chunks

        // Delete
        [Fact]
        public async Task DeleteByIdUsingAPI_send_int_Id_get_bool_true()
        {
            //Arrange
            var options = CreateNewContextOptions();
            var appDbOptions = CreateNewApplicationDbContextOptions();
            FillTheTempDataBase(options);
            AddDataChunksToExistingFileItemToDB(options);

            int FileItemId = 11;

            using (var appDbContext = new ApplicationDbContext(appDbOptions))
            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context, appDbContext);

                //Act  
                var result = await service.DeleteByIdUsingAPI(FileItemId);

                //Assert
                Assert.Equal(3, context.FileItems.Count());
                Assert.Equal(0, context.DataChunks.Count());
                var viewResult = Assert.IsType<bool>(result);
                Assert.True(result);

            }
        }

        #endregion

        #region File Item & datachunks

        // GetSpecificFilItemAndDataChunks. Return URI 
        [Fact]
        public async Task GetSpecificFilItemAndDataChunks_send_id_and_userId_get_URI()
        {
            //Arrange
            var options = CreateNewContextOptions();
            var appDbOptions = CreateNewApplicationDbContextOptions();
            FillTheTempDataBase(options);
            AddDataChunksToExistingFileItemToDB(options);
            int FileItemId = 11;
            string userGuid = "User-1a-guid-tostring";

            using (var appDbContext = new ApplicationDbContext(appDbOptions))
            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context, appDbContext);

                //Act  
                var result = await service.GetSpecificFilItemAndDataChunks(FileItemId, userGuid);

                //Assert
                Assert.Equal(2, context.DataChunks.Count());
                Assert.Equal(11, context.DataChunks.FirstOrDefault().FileItemId);
                Assert.Equal(11, context.DataChunks.FirstOrDefault().Id);
                var viewResults = Assert.IsType<Uri>(result);

            }
        }

        // GetSpecificFilItemAndDataChunks. Return FileItem with include DataChunks
        [Fact]
        public async Task GetFiAndDc_send_id_and_userId_get_FileItem()
        {
            //Arrange
            var options = CreateNewContextOptions();
            var appDbOptions = CreateNewApplicationDbContextOptions();
            FillTheTempDataBase(options);
            AddDataChunksToExistingFileItemToDB(options);
            int FileItemId = 11;
            string userGuid = "User-1a-guid-tostring";

            using (var appDbContext = new ApplicationDbContext(appDbOptions))
            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context, appDbContext);

                //Act  
                var result = await service.GetSpecifikFileItemAndDataChunk(FileItemId, userGuid);

                //Assert
                Assert.Equal(2, context.DataChunks.Count());
                Assert.Equal(11, context.DataChunks.FirstOrDefault().FileItemId);
                Assert.Equal(11, context.DataChunks.FirstOrDefault().Id);
                var viewResults = Assert.IsType<FileItem>(result);
                Assert.Equal(2, viewResults.DataChunks.Count());

            }
        }

        // GetAllFilItemAndDataChunks (ALL)
        [Fact]
        public async Task GetAllFilItemAndDataChunks__userId_get_FileItem()
        {
            //Arrange
            var options = CreateNewContextOptions();
            var appDbOptions = CreateNewApplicationDbContextOptions();
            AddInitFileItemToDb(options);

            AddDataChunksToDB(options);

            string userGuid = "111cf2f2-c675-4e27-ac7a-9f8e43f64334";


            using (var appDbContext = new ApplicationDbContext(appDbOptions))
            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context, appDbContext);

                //Act  
                var result = await service.GetAllFilItemAndDataChunks(userGuid);

                //Assert
                Assert.Equal(2, context.DataChunks.Count());
                var viewResult = Assert.IsType<List<FileItem>>(result);
                Assert.Equal(2, viewResult.FirstOrDefault().DataChunks.Count());

            }
        }

        #endregion

        #region check checksum DataChunks

        //check if checksum exist. There is no exisisting datachunk with this checksum. User has fileitem saved, user has datachunks saved.
        [Fact]
        public async Task CheckChecksum_send_userId_and_checksum_to_see_if_checksum_exist()
        {
            //Arrange
            var options = CreateNewContextOptions();
            var appDbOptions = CreateNewApplicationDbContextOptions();
            FillTheTempDataBase(options);
            AddDataChunksToExistingFileItemToDB(options);
            string TestUserID = "User-1a-guid-tostring"; // this user exist and has fileitem with datachunks saved. 
            string TestDatachunkChecksum = "cc33-checksum-fil"; //aa11-checksum-fil & bb22-checksum-fil exist.

            using (var appDbContext = new ApplicationDbContext(appDbOptions))
            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context, appDbContext);

                //Act  
                var result = await service.CheckChecksum(TestUserID, TestDatachunkChecksum);

                //Assert
                Assert.Equal(2, context.DataChunks.Count());
                var viewResult = Assert.IsType<bool>(result);
                Assert.False(viewResult);

            }
        }

        //check if checksum exist. User has not added any chunks so there's not even any datachunks.
        [Fact]
        public async Task CheckChecksum_check_if_checksum_exist_there_are_no_datachunks_saved()
        {
            //Arrange
            var options = CreateNewContextOptions();
            var appDbOptions = CreateNewApplicationDbContextOptions();
            FillTheTempDataBase(options);
            AddDataChunksToExistingFileItemToDB(options);
            string TestUserID = "User-2a-guid-tostring"; // this user exist and has fileitem but no datachunks saved. 
            string TestDatachunkChecksum = "aa11-checksum-fil"; //aa11-checksum-fil exsist but in another users datachunk

            using (var appDbContext = new ApplicationDbContext(appDbOptions))
            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context, appDbContext);

                //Act  
                var result = await service.CheckChecksum(TestUserID, TestDatachunkChecksum);

                //Assert
                Assert.Equal(2, context.DataChunks.Count());
                var viewResult = Assert.IsType<bool>(result);
                Assert.False(viewResult);

            }
        }

        //check if checksum exist. The checksum does exsist, expected return is true.
        [Fact]
        public async Task CheckChecksum_the_checksum_exsists_return_true()
        {
            //Arrange
            var options = CreateNewContextOptions();
            var appDbOptions = CreateNewApplicationDbContextOptions();
            FillTheTempDataBase(options);
            AddDataChunksToExistingFileItemToDB(options);
            string TestUserID = "User-1a-guid-tostring"; // this user exist and has fileitem and datachunks saved. 
            string TestDatachunkChecksum = "aa11-checksum-fil"; //aa11-checksum-fil & bb22-checksum-fil exist.

            using (var appDbContext = new ApplicationDbContext(appDbOptions))
            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context, appDbContext);

                //Act  
                var result = await service.CheckChecksum(TestUserID, TestDatachunkChecksum);

                //Assert
                Assert.Equal(2, context.DataChunks.Count());
                var viewResult = Assert.IsType<bool>(result);
                Assert.True(viewResult);

            }
        }

        #endregion

        #region check checksum FileItem
        //Kolla användarens fileitems, i detta fall finns redan filen genom att kolla checksum. 
        [Fact]
        public async Task CheckChecksumOnFileItem_the_checksum_exists_return_true()
        {
            //Arrange
            var options = CreateNewContextOptions();
            var appDbOptions = CreateNewApplicationDbContextOptions();
            FillTheTempDataBase(options);
            AddDataChunksToExistingFileItemToDB(options);
            string TestUserID = "User-1a-guid-tostring"; // this user exist and has fileitem and datachunks saved. 
            string TestDatachunkChecksum = "aaa111checksum"; //fileitems checksum.

            using (var appDbContext = new ApplicationDbContext(appDbOptions))
            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context, appDbContext);

                //Act  
                var result = await service.CheckChecksumOnFileItem(TestUserID, TestDatachunkChecksum);

                //Assert
                Assert.Equal(2, context.DataChunks.Count());
                var viewResult = Assert.IsType<bool>(result);
                Assert.True(viewResult);

            }
        }

        //Kolla användarens fileitems, i detta fall finns inte denna checksum
        [Fact]
        public async Task CheckChecksumOnFileItem_the_checksum_does_not_exists_return_fasle()
        {
            //Arrange
            var options = CreateNewContextOptions();
            var appDbOptions = CreateNewApplicationDbContextOptions();
            FillTheTempDataBase(options);
            AddDataChunksToExistingFileItemToDB(options);
            string TestUserID = "User-1a-guid-tostring"; // this user exist and has fileitem and datachunks saved. 
            string TestDatachunkChecksum = "xxx999checksum"; //fileitems checksum.

            using (var appDbContext = new ApplicationDbContext(appDbOptions))
            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context, appDbContext);

                //Act  
                var result = await service.CheckChecksumOnFileItem(TestUserID, TestDatachunkChecksum);

                //Assert
                Assert.Equal(2, context.DataChunks.Count());
                var viewResult = Assert.IsType<bool>(result);
                Assert.False(viewResult);

            }
        }

        // En FileItem har försökts läggas till, fast checksum säger att filen redan finns.
        // Kollar att antalet datachunks som ska finnas finns, return true.
        [Fact]
        public async Task DoesAllChunksExist_count_datachunks_count_and_actual_count_is_same_return_true()
        {
            //Arrange
            var options = CreateNewContextOptions();
            var appDbOptions = CreateNewApplicationDbContextOptions();
            FillTheTempDataBase(options);
            AddDataChunksToExistingFileItemToDB(options);
            int fileItemID = 11;

            using (var appDbContext = new ApplicationDbContext(appDbOptions))
            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context, appDbContext);

                //Act  
                var result = await service.DoesAllChunksExist(fileItemID);

                //Assert
                Assert.Equal(2, context.DataChunks.Count());
                var viewResult = Assert.IsType<bool>(result);
                Assert.True(viewResult);

            }
        }

        // Kollar att antalet datachunks som ska finnas finns, return true.
        [Fact]
        public async Task DoesAllChunksExist_count_datachunks_count_and_actual_count_is__Not_same_return_false()
        {
            //Arrange
            var options = CreateNewContextOptions();
            var appDbOptions = CreateNewApplicationDbContextOptions();
            FillTheTempDataBase(options);
            AddCorruptDataChunksToExistingFileItemToDB(options); // partname säger att det ska finnas 3 parts, fast det finns bara 2.
            int fileItemID = 11;

            using (var appDbContext = new ApplicationDbContext(appDbOptions))
            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context, appDbContext);

                //Act  
                var result = await service.DoesAllChunksExist(fileItemID);

                //Assert
                Assert.Equal(2, context.DataChunks.Count());
                var viewResult = Assert.IsType<bool>(result);
                Assert.False(viewResult);

            }
        }

        #endregion
    }
}
