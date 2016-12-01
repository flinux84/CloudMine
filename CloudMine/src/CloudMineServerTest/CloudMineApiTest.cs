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

        private void FillTheTempDataBase(DbContextOptions<CloudDbRepository> options)
        {
            using (var context = new CloudDbRepository(options))
            {
                //context.dbFileItem.Add(new Models.FileItem { Id = 10, Created = new DateTime(2010, 1, 01), DataType = "jpg", Description = "I realy know how to take pictures.", FileName = "My image", FileSize = 1, Private = false, UserId = 15, FileData = new byte[10] });
                //context.dbFileItem.Add(new Models.FileItem { Id = 33, Created = new DateTime(2011, 2, 10), DataType = "mp4", Description = "A real good song that i like to listen to.", FileName = "Good song", FileSize = 2, Private = true, UserId = 15, FileData = new byte[1010] });
                //context.dbFileItem.Add(new Models.FileItem { Id = 4455, Created = new DateTime(2015, 3, 20), DataType = "doc", Description = "Some time a take notes.", FileName = "Evening notes", FileSize = 3, Private = false, UserId = 30, FileData = new byte[101010] });
                //context.dbFileItem.Add(new Models.FileItem { Id = 6677, Created = new DateTime(2015, 3, 20), DataType = "pdf", Description = "x", FileName = "Bif file", FileSize = 3, Private = false, UserId = 30, FileData = new byte[101010], FileChunkId = 11, FileChunkIndex=0});
                //context.dbFileItem.Add(new Models.FileItem { Id = 7766, Created = new DateTime(2015, 3, 20), DataType = "pdf", Description = "x", FileName = "Bif file", FileSize = 3, Private = false, UserId = 30, FileData = new byte[101010], FileChunkId = 11, FileChunkIndex = 1 });
                //context.SaveChanges();
            }
        }

        private void AddInitFileItemToDb(DbContextOptions<CloudDbRepository> options)
        {
            using (var context = new CloudDbRepository(options))
            {
                Guid FileItemGuid = new Guid("976cf2f2-c675-4e27-ac7a-9f8e43f64334");
                Guid userGuid = new Guid("111cf2f2-c675-4e27-ac7a-9f8e43f64334");
                context.FileItems.Add(new FileItem { Id = 1, FileItemId = FileItemGuid, UserId = userGuid, DataChunk = null, Private = true, FileSize = 111, FileName = "TEST", Description = "test", DataType = "jpg" });
                context.SaveChanges();
            }
        }

        private void AddDataChunksToDB(DbContextOptions<CloudDbRepository> options)
        {
            using (var context = new CloudDbRepository(options))
            {
                Guid FileItemGuid = new Guid("976cf2f2-c675-4e27-ac7a-9f8e43f64334");
                context.DataChunks.Add(new DataChunk { Id = 0, FileItemId = FileItemGuid, Data = new byte[10], PartIndex = 1 });
                context.DataChunks.Add(new DataChunk { Id = 0, FileItemId = FileItemGuid, Data = new byte[01], PartIndex = 2 });
                context.SaveChanges();
            }
        }


        //private static FileItemSet GetFileItemSetToTestMethod()
        //{
        //    List<FileItem> fiList = new List<FileItem>();
        //    fiList.Add(new Models.FileItem { Id = 5555, Created = new DateTime(2016, 1, 01), DataType = "jpg", Description = "Look at this picture", FileName = "Image", FileSize = 1, Private = false, UserId = 50, FileData = new byte[10] });
        //    fiList.Add(new Models.FileItem { Id = 8888, Created = new DateTime(2016, 2, 10), DataType = "mp4", Description = "Realy good song.", FileName = "Music", FileSize = 2, Private = true, UserId = 50, FileData = new byte[1010] });
        //    FileItemSet fis = new FileItemSet() { UserId = 88, ListFileItems = fiList };
        //    return fis;
        //}
        //private static FileItemSet GetBadFileItemSetToTestMethod()
        //{
        //    //List<FileItem> fiList = new List<FileItem>();
        //    //fiList.Add(new Models.FileItem { Id = 10, Created = new DateTime(2010, 1, 01), DataType = "jpg", Description = "I realy know how to take pictures.", FileName = "My image", FileSize = 1, Private = false, UserId = 15, FileData = new byte[10] });
        //    //fiList.Add(new Models.FileItem { Id = 10, Created = new DateTime(2010, 1, 01), DataType = "jpg", Description = "I realy know how to take pictures.", FileName = "My image", FileSize = 1, Private = false, UserId = 15, FileData = new byte[10] });
        //    //FileItemSet fis = new FileItemSet() { UserId = 88, ListFileItems = fiList };
        //    //return fis;
        //}
        //private FileItemSet GetFileItemSetEmptyList()
        //{
        //    //FileItemSet fis = new FileItemSet();
        //    //fis.UserId = 15;
        //    //return fis;
        //}
        //private FileItem GetFileItemToEdit()
        //{
        //    ////user id 15. change datetime, discription, filedata.
        //    //FileItem fi = new FileItem() { Id = 10, Created = new DateTime(2016, 11, 24), DataType = "jpg", Description = "Realy not that good.", FileName = "My image", FileSize = 1, Private = false, UserId = 15, FileData = new byte[1010] };

        //    //return fi;
        //}
        #endregion

        //#region Testarea: CRUD

        [Fact]
        public async Task InitCreateFileItem_add_initial_FileItem_to_db()
        {
            //Arrange
            var options = CreateNewContextOptions();
            var fis = new FileItem() { Private = true, FileSize = 111, FileName = "TEST", Description = "test", DataType = "jpg" };

            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context);

                //Act  
                var result = await service.InitCreateFileItem(fis);

                //Assert
                var viewResult = Assert.IsType<string>(result);
                Assert.Equal(1, context.FileItems.Count());
                Assert.Equal(viewResult, context.FileItems.FirstOrDefault().FileItemId.ToString());
            }
        }


        // Create
        [Fact]
        public async Task AddFileUsingAPI_add_file_and_get_true_sucess_save_to_db()
        {
            //Arrange
            var options = CreateNewContextOptions();
            AddInitFileItemToDb(options);
            Guid myGuid = new Guid("976cf2f2-c675-4e27-ac7a-9f8e43f64334");
            var ds = new DataChunk() { Id = 0, FileItemId = myGuid, Data = new byte[10], PartIndex = 1 };

            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context);

                //Act  
                var result = await service.AddFileUsingAPI(ds);

                //Assert
                var viewResult = Assert.IsType<string>(result);
                Assert.Equal("Ok", viewResult);
                Assert.Equal(myGuid, context.DataChunks.FirstOrDefault().FileItemId);
                Assert.Equal(1, context.FileItems.Count());
            }
        }

        // TODO : fail AddFileUsingAPI

        // Create
        //[Fact]
        //public async Task AddFileUsingAPI_add_file_and_get_false_catch_save_to_db()
        //{
        //    //Arrange
        //    var options = CreateNewContextOptions();
        //    FillTheTempDataBase(options);
        //    var fis = GetBadFileItemSetToTestMethod();

        //    using (var context = new CloudDbRepository(options))
        //    {
        //        var service = new CloudMineDbService(context);

        //        //Act  
        //        var result = await service.AddFileUsingAPI(fis);

        //        //Assert
        //        var viewResult = Assert.IsType<bool>(result);
        //        Assert.False(result);
        //    }
        //}

        // Read (All)
        [Fact]
        public async Task GetAllFilesUsingAPI_get_all_the_users_files_get_a_FileItemSet()
        {
            //Arrange
            var options = CreateNewContextOptions();
            AddInitFileItemToDb(options);
            Guid userGuid = new Guid("111cf2f2-c675-4e27-ac7a-9f8e43f64334");


            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context);

                //Act  
                var result = await service.GetAllFilesUsingAPI(userGuid);

                //Assert
                var viewResult = Assert.IsType<FileItemSet>(result);
                Assert.Equal(1, context.FileItems.Count());
                Assert.Equal("TEST", context.FileItems.FirstOrDefault().FileName);
            }
        }

        //// Read (All)
        //[Fact]
        //public async Task GetAllFilesUsingAPI_FileItemSet_is_not_empty_return_sender_FileItemSet()
        //{
        //    //Arrange
        //    var options = CreateNewContextOptions();
        //    FillTheTempDataBase(options);
        //    var fis = GetBadFileItemSetToTestMethod();

        //    using (var context = new ApplicationDbContext(options))
        //    {
        //        var service = new CloudMineDbService(context);

        //        //Act  
        //        var result = await service.GetAllFilesUsingAPI(fis);

        //        //Assert
        //        var viewResult = Assert.IsType<FileItemSet>(result);

        //    }
        //}

        // Read (One) - about to be deprecated (Tänkt att ersättas av: GetFileChunsByIdAndUserId)
        [Fact]
        public async Task GetFileByIdUsingAPI_send_int_id_get_FileItem_back()
        {
            //Arrange
            var options = CreateNewContextOptions();
            AddInitFileItemToDb(options);
            int FileItemId = 1;

            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context);

                //Act  
                var result = await service.GetFileByIdUsingAPI(FileItemId);

                //Assert
                var viewResult = Assert.IsType<FileItem>(result);
                Assert.Equal(1, context.FileItems.Count());
                Assert.Equal("TEST", viewResult.FileName);
            }
        }

        //// Read One - Get all chuncks from same file
        //[Fact]
        //public async Task GetFileChunsByIdAndUserId_send_int_id_send_userId_get_FileItemSet_back()
        //{
        //    //Arrange
        //    var options = CreateNewContextOptions();
        //    FillTheTempDataBase(options);
        //    int userId = 30;
        //    int filId = 6677;

        //    using (var context = new ApplicationDbContext(options))
        //    {
        //        var service = new CloudMineDbService(context);

        //        //Act  
        //        var result = await service.GetFileChunsByIdAndUserId(filId, userId);

        //        //Assert
        //        var viewResult = Assert.IsType<FileItemSet>(result);

        //    }
        //}

        // Update
        [Fact]
        public async Task UpDateByIdUsingAPI_send_int_id_and_FileItem_maching_id_return_true()
        {
            //Arrange
            var options = CreateNewContextOptions();
            AddInitFileItemToDb(options);
            var myFileItem = new FileItem() { Id = 1, Private = false, FileSize = 111, FileName = "EDIT", Description = "edit", DataType = "jpg" };
            int FileItemId = 1;

            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context);

                //Act  
                var result = await service.UpDateByIdUsingAPI(FileItemId, myFileItem);

                //Assert
                var viewResult = Assert.IsType<bool>(result);
                Assert.True(result);
                Assert.Equal(1, context.FileItems.Count());
                Assert.Equal("EDIT", context.FileItems.FirstOrDefault().FileName);
            }
        }

        //// Update
        //[Fact]
        //public async Task UpDateByIdUsingAPI_send_int_id_and_FileItem_that_dont_maching_id_return_false()
        //{
        //    //Arrange
        //    var options = CreateNewContextOptions();
        //    FillTheTempDataBase(options);
        //    var myFileItem = GetFileItemToEdit();
        //    int FileItemId = 11;

        //    using (var context = new ApplicationDbContext(options))
        //    {
        //        var service = new CloudMineDbService(context);

        //        //Act  
        //        var result = await service.UpDateByIdUsingAPI(FileItemId, myFileItem);

        //        //Assert
        //        var viewResult = Assert.IsType<bool>(result);
        //        Assert.False(result);
        //    }
        //}

        // Delete
        [Fact]
        public async Task DeleteByIdUsingAPI_send_int_Id_get_bool_true()
        {
            //Arrange
            var options = CreateNewContextOptions();
            AddInitFileItemToDb(options);
            int FileItemId = 1;

            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context);

                //Act  
                var result = await service.DeleteByIdUsingAPI(FileItemId);

                //Assert
                var viewResult = Assert.IsType<bool>(result);
                Assert.True(result);
                Assert.Equal(0, context.FileItems.Count());
            }
        }

        //#endregion


        #region File Item & datachunks

        // GetSpecificFilItemAndDataChunks

        [Fact]
        public async Task GetSpecificFilItemAndDataChunks_send_id_and_userId_get_FileItem()
        {
            //Arrange
            var options = CreateNewContextOptions();
            AddInitFileItemToDb(options);
            AddDataChunksToDB(options);
            int FileItemId = 1;
            Guid userGuid = new Guid("111cf2f2-c675-4e27-ac7a-9f8e43f64334");
            Guid FileItemGuid = new Guid("976cf2f2-c675-4e27-ac7a-9f8e43f64334");

            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context);

                //Act  
                var result = await service.GetSpecificFilItemAndDataChunks(FileItemId);

                //Assert
                Assert.Equal(2, context.DataChunks.Count());
                Assert.Equal(FileItemGuid, context.DataChunks.FirstOrDefault().FileItemId);
                Assert.Equal(1, context.DataChunks.FirstOrDefault().Id);

                var viewResult = Assert.IsType<FileItem>(result);
                Assert.Equal(2, viewResult.DataChunk.Count());
            }
        }

        // GetAllFilItemAndDataChunks
        [Fact]
        public async Task GetAllFilItemAndDataChunks__userId_get_FileItem()
        {
            //Arrange
            var options = CreateNewContextOptions();
            AddInitFileItemToDb(options);
            AddDataChunksToDB(options);
          
            Guid userGuid = new Guid("111cf2f2-c675-4e27-ac7a-9f8e43f64334");
        

            using (var context = new CloudDbRepository(options))
            {
                var service = new CloudMineDbService(context);

                //Act  
                var result = await service.GetAllFilItemAndDataChunks(userGuid);

                //Assert
                Assert.Equal(2, context.DataChunks.Count());
                var viewResult = Assert.IsType<List<FileItem>>(result);
                Assert.Equal(2, viewResult.FirstOrDefault().DataChunk.Count());
            }
        }

        #endregion

    }
}
