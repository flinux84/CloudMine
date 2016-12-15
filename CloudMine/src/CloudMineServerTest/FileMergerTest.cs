using CloudMineServer.Models;
using CloudMineServer.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CloudMineServerTest
{
    public class FileMergerTest
    {
        //Arrange
        private FileItem CreateFileItem(string filename)
        {
            var chunk1 = new DataChunk() {
                Id = 1,
                Data = new byte[] { },
                FileItemId = 1,
                PartName = "banan.png.part_1.2",
            };

            var chunk2 = new DataChunk() {
                Id = 2,
                Data = new byte[] { },
                FileItemId = 1,
                PartName = "banan.png.part_2.2",
            };

            var fileitem = new FileItem() {
                Id = 1,
          //      Checksum = new Guid("BE043E20-DBCD-4CA2-9000-0B2AB8BAE19E"),
                DataType = ".png",
                Description = "En bild",
                FileName = "banan.png",
                FileSize = 50000,
                IsComplete = true,
                Uploaded = new DateTime(2016, 12, 5),
                UserId = "2C898E3F-F909-4F23-AA4F-72320431A604",
                DataChunks = new List<DataChunk>() { chunk1, chunk2 }            
            };

            return fileitem;
        }



        ////[Fact]
        ////public void FileMergOK()
        ////{
        ////    //Arrange
        ////    var merger = new FileMerge();

        ////    //Act
        ////    var uri = merger.MakeFile(CreateFileItem("banan.png"));
        ////    var test = uri.AbsolutePath;

        ////    //Assert
        ////    Assert.IsAssignableFrom<Uri>(uri);
        ////    Assert.NotNull(uri);
        ////}

        //[Fact]
        //public async void FileMergeThrowsException()
        //{
        //    //Arrange
        //    var merger = new FileMerge();

        //    //Act - send in a filename that doesnt correspond with the chunks in the fileitem.
        //    Uri u;
        //    var exception = await Record.ExceptionAsync(() => u = merger.MakeFile(CreateFileItem("citron.png")));

        //    //Assert
        //    Assert.IsAssignableFrom<InvalidOperationException>(exception);            

        //}
    }
}
