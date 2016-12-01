using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CloudMineServer.Data;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using CloudMineServer.Models;
using CloudMineServer.Interface;
using System;
using System.Linq;
using CloudMineServer.Classes;
using CloudMineServer.Services;

namespace CloudMineServer.Controllers
{
    [Route("api/filechunk/")]
    public class InternalAPI : Controller
    {
        //Linus testa-runt-klass
        private CloudDbRepository context;
        public InternalAPI(CloudDbRepository context)
        {
            this.context = context;
        }

        //[HttpPost]
        //public async Task<IActionResult> UploadMetaData()
        //{
        //    return null;
        //}

        //[HttpGet]
        //public async Task<IActionResult> Return(string filename)
        //{
        //    var fileitem = new FileItem();
        //    fileitem.FileName = "StorageExplorer.exe";
        //    fileitem.DataChunks = context.DataChunks.Where(s => s.PartName.StartsWith("Storage")).ToList();
        //    FileMerge merge = new FileMerge();
        //    var retur = merge.MakeFile(fileitem);

        //    //return new VirtualFileResult(retur.AbsolutePath, "application/octet-stream");
        //    return new PhysicalFileResult(retur.AbsolutePath, "application/octet-stream");
        //    //return new PhysicalFileResult(retur.AbsolutePath, "application/octet-stream");
        //    //return new FileStreamResult(stream, "application/octet-stream");
        //}

        //[HttpPost]
        //public async Task<IActionResult> UploadFileChunk()
        //{
        //    for (int i = 0; i < Request.Form.Files.Count; i++)
        //    {
        //        var myFile = Request.Form.Files[i];
        //        if (myFile != null && myFile.Length != 0)
        //        {

        //            var chunk = new DataChunk() {
        //                PartName = myFile.FileName,
        //                Data = StreamToArray(myFile.OpenReadStream()),
        //                FileItemId = 2
        //            };

        //            context.DataChunks.Add(chunk);
        //            try
        //            {
        //                context.SaveChanges();
        //            } catch (Exception e)
        //            {

        //                throw new Exception(e.Message);
        //            }
                    
        //        }
        //    }


        //    return new ObjectResult("hejsan");
        //}

        //private static byte[] StreamToArray(Stream input)
        //{
        //    byte[] buffer = new byte[16 * 1024];
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        int read;
        //        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
        //        {
        //            ms.Write(buffer, 0, read);
        //        }
        //        return ms.ToArray();
        //    }
        //}

        //private void MergeFileChunks(List<FileItem> chunkedfile)
        //{

        //}
    }
}
