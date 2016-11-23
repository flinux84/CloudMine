using CloudMineServer.Data;
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
        #region Repository
        //byggt en test databas: 
        private static DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
        {
            //setupmetod. ingen riktig databas och vet inte om rellationer osv
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

        private void FillTheTempDataBase(DbContextOptions<ApplicationDbContext> options)
        {
            using (var context = new ApplicationDbContext(options))
            {
                context.dbFileItem.Add(new Models.FileItem { Id = 10, Created = new DateTime(2010, 1, 01), DataType = "jpg", Description = "I realy know how to take pictures.", FileName = "My image", FileSize = 1, Private = false, UserId = 15, FileData = new byte[10] });
                context.dbFileItem.Add(new Models.FileItem { Id = 33, Created = new DateTime(2011, 2, 10), DataType = "mp4", Description = "A real good song that i like to listen to.", FileName = "Good song", FileSize = 2, Private = true, UserId = 15, FileData = new byte[1010] });
                context.dbFileItem.Add(new Models.FileItem { Id = 4455, Created = new DateTime(2015, 3, 20), DataType = "doc", Description = "Some time a take notes.", FileName = "Evening notes", FileSize = 3, Private = false, UserId = 30, FileData = new byte[101010] });

                context.SaveChanges();
            }
        }
        #endregion 



    }
}
