using System;
using CloudMineServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudMineServer.Models;
using Xunit;
using Microsoft.AspNetCore.Mvc;

//namespace CloudMineServerTest
namespace CloudMineServer.Controllers
{
    public class RestTest
    {
        #region setup fake inerface

        private static Interface.ICloudMineApi IFakeBusinessLayer()
        {
            var fc = new FakeClass();
            return fc;
        }

        #endregion

       // [Fact]
        public async Task cant_find_test_in_testexplorer()
        {
            //Arrange
            var options = IFakeBusinessLayer();
            var fis = new FileItemSet();

            using (var context = new Rest(options))
            {

                //Act 
                var result = await context.UploadFileSet(fis);

                //Assert
                var viewResult = Assert.IsType<ObjectResult>(result); 
              
            }
        }

     //   [Fact]
        public async Task just_make_a_test_run()
        {
            //Arrange
            FileItemSet asdf = new FileItemSet();
            var controller = new Rest(IFakeBusinessLayer());
            //Act
            var result = await controller.UploadFileSet(asdf);
            //Assert
            var viewResult = Assert.IsType<ObjectResult>(result);
        }

        private class FakeClass : Interface.ICloudMineApi
        {
            #region Interface
            public async Task<bool> AddFileUsingAPI(FileItemSet FIS)
            {
                return true;
            }

            public async Task<bool> DeleteByIdUsingAPI(int num)
            {
                return true;
            }

            public async Task<FileItemSet> GetAllFilesUsingAPI(FileItemSet item)
            {
                FileItemSet fis = new FileItemSet();
                return fis;
            }

            public async Task<FileItem> GetFileByIdUsingAPI(int num)
            {
                FileItem fi = new FileItem();
                return fi;
            }

            public async Task<bool> UpDateByIdUsingAPI(int num, FileItem item)
            {
                return true;
            }
            #endregion

            
        }
    }
}
