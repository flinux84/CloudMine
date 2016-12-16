using CloudMineServer.API_server.Models;
using CloudMineServer.API_server.Services;
using CloudMineServer.Interface;
using CloudMineServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Text.RegularExpressions;
using CloudMineServer.Models;

namespace CloudMineServer.API_server.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/GetFile")]
    public class Returnfile : Controller
    {
        private ICloudMineDbService _context;
        public Returnfile(ICloudMineDbService context)
        {
            _context = context;
        }


        //Denna fungerar enligt "gamla metoden". Filen skapas på servern och returneras till klienten.
        //dock ligger filen kvar på servern så den tas inte bort, kanske går att lägga till funktion för det.
        [HttpGet("{id:int}")]
        public async Task<FileStreamResult> Get([FromRoute] int id)
        {
            var merge = new FileMerge();
            var userId = User.GetUserId();

            var file = await _context.GetSpecifikFileItemAndDataChunk(id, userId);

            var merger = new FileMerge();
            var uri = merger.MakeFileOnServer(file);

            var stream = System.IO.File.OpenRead(uri.AbsolutePath);

            return new FileStreamResult(stream, new MediaTypeHeaderValue("application/octet-stream"))
            {
                FileDownloadName = file.FileName
            };

        }

        //Martins förslag bygger på att man använder en ny returtyp som ärver FileResult.
        //Denna har jag lagt i services och heter FileCallbackResult och ska kunna hantera chunks av streams.
        //Så då skulle man isåfall implementera en annan Get-metod som använder denna och vi skippar isåfall att använda FileMerge().
        //Länk att läsa på detta: http://blog.stephencleary.com/2016/11/streaming-zip-on-aspnet-core.html
        // i länken så skapar han upp en zip-fil "on the fly", men vi får anpassa den isåfall.


        // Uploads file without saving on server disk.
        // Supports resume if browser/downloader has support
        // GET: api/v{version:apiVersion}/GetFile/NoDisk/id
        [Authorize]
        [HttpGet("NoDisk/{id:int}")]
        public async Task<IActionResult> GetFileNoDisk([FromRoute]int id)
        {
            var fileItem = await _context.GetFileByIdUsingAPI(id);
            if (fileItem == null)
                return BadRequest("File does not exist");

            var dataChunk = await _context.GetFirstDataChunk(id);

            StringValues rangeValues;
            // The Range header indicates this is a resume request
            if (Request.Headers.TryGetValue("Range", out rangeValues))
            {
                int startByteNr = 0;
                var startByteString = Regex.Match(rangeValues.First(), @"\d+").Value;
                int.TryParse(startByteString, out startByteNr);

                dataChunk = await GetResumeDataChunk(dataChunk, fileItem, startByteNr);

                Response.StatusCode = 206;
                Response.Headers.Add("Content-Length", (fileItem.FileSize - startByteNr).ToString());
                Response.Headers.Add("Content-Range", $"bytes {startByteNr}-{fileItem.FileSize - 1}/{fileItem.FileSize}");
            }
            else
            {
                Response.Headers.Add("Content-Length", fileItem.FileSize.ToString());
            }
            Response.Headers.Add("Accept-Ranges", "bytes");
            Response.Headers.Add("Connection", "keep-alive");
            Response.Headers.Add("Transfer-Encoding", "");

            return new FileCallbackResult(new MediaTypeHeaderValue("application/octet-stream"), async (outputStream, _) =>
            {
                while (dataChunk != null)
                {
                    using (Stream readStream = new MemoryStream(dataChunk.Data))
                    {
                        await readStream.CopyToAsync(outputStream);
                    }
                    dataChunk = await _context.GetNextDataChunk(dataChunk);
                }
            })
            { FileDownloadName = fileItem.FileName };

            #region OldAndTest
            //var dataChunks = fileItem.DataChunks.ToList();
            //dataChunks.Sort(new DataChunkPartNameComparer());

            //return new FileCallbackResult(new MediaTypeHeaderValue("application/octet-stream"), async (outputStream, _) =>
            //{
            //    foreach (var dataChunk in dataChunks)
            //    {
            //        using (Stream readStream = new MemoryStream(dataChunk.Data))
            //        {
            //            await readStream.CopyToAsync(outputStream);
            //        }
            //    }
            //})
            //{ FileDownloadName = fileItem.FileName };


            //var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            //List<byte[]> byteArrays = new List<byte[]>
            //{
            //    new byte[]{ 1, 2, 3, 4, 5, 6, 7, 8 },
            //    new byte[]{ 9, 10, 11, 12, 13, 14, 15, 16 },
            //    new byte[]{ 17, 18, 19, 20, 21, 22, 23, 24 },
            //    new byte[]{ 25, 26, 27, 28, 29, 30, 31, 32 }
            //};

            //return new FileCallbackResult(new MediaTypeHeaderValue("application/octet-stream"), async (outputStream, _) =>
            //{
            //    foreach (var ba in byteArrays)
            //    {
            //        using (Stream readStream = new MemoryStream(ba))
            //        {
            //            await readStream.CopyToAsync(outputStream);
            //        }
            //    }
            //})
            //{ FileDownloadName = "TestBytes.txt"};
            #endregion
        }

        // Finds the DataChunk to resume from.
        // Skips required nr of bytes in that chunk.
        private async Task<DataChunk> GetResumeDataChunk(
            DataChunk dataChunk, FileItem fileItem, int startByteNr)
        {
            int chunkSize = dataChunk.Data.Count();
            if (fileItem.FileSize < dataChunk.Data.Count())
            {
                dataChunk.Data = 
                    dataChunk.Data.Skip(startByteNr).ToArray();
            }
            else
            {
                int indexOfStartChunk = (int)(Math.Floor(
                    ((float)startByteNr / (float)fileItem.FileSize)
                    * (float)dataChunk.NumberOfChunksInSequence()));

                // dataChunk to resume from
                dataChunk = await _context.GetDataChunkAtIndex(dataChunk, indexOfStartChunk);

                // Skip ahead to start byte in dataChunk
                dataChunk.Data = dataChunk.Data.Skip(
                    startByteNr - indexOfStartChunk * chunkSize).ToArray();

                // If we skipped all bytes start resume from the begining of next chunk
                if (dataChunk.Data.Count() == 0)
                    dataChunk = await _context.GetNextDataChunk(dataChunk);
            }
            return dataChunk;
        }


    }
}
