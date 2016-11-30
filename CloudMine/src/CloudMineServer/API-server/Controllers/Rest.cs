using CloudMineServer.Classes;
using CloudMineServer.Interface;
using CloudMineServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CloudMineServer.Controllers
{
    //API VERSIONING
    [Authorize]
    [ApiVersion( "1.0" )]
    [Route( "api/v{version:apiVersion}/[controller]" )]
    public class Rest : Controller
    {
        #region Dependency Injection Constructor

        private readonly ICloudMineDbService _businessLayer;

        public Rest(ICloudMineDbService businessLayer)
        {
            _businessLayer = businessLayer;
        }

        #endregion

        #region CRUD

        // Create
        [HttpPost]
        public async Task<IActionResult> UploadFileSet([FromBody]FileItemSet item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            bool add = await _businessLayer.AddFileUsingAPI(item);

            if (add)
            {
                return new ObjectResult("Sucess");
            }
            else
            {
                return new ObjectResult("Api error - when trying to add object.");
            }
        }

        // Read
        [HttpGet]
        public async Task<IActionResult> GetAllFiles([FromBody]FileItemSet item) // tar emot FileItemSet mest bara för att kunna avgöra vem som frågar om att få lista med filer. 
        {

            FileItemSet returnList = await _businessLayer.GetAllFilesUsingAPI(item);

            //var json = JsonConvert.SerializeObject(returnList);

            //return json;

            return new ObjectResult(returnList);
        }

        [HttpGet("{id}", Name = "GetRest")]
        public async Task<IActionResult> GetFileById(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                FileItem item = await _businessLayer.GetFileByIdUsingAPI(id);
                if (item == null)
                {
                    return new ObjectResult("object not found");
                }
                return new ObjectResult(item);
            }
            else
            {
                return new ObjectResult("object available");
            }
        }

        // Update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpDateById(string id, [FromBody] FileItem item)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                bool update = await _businessLayer.UpDateByIdUsingAPI(id, item);
                if (update)
                {
                    return new ObjectResult("Success");
                }
                else
                {
                    return new ObjectResult("Api error - when trying to update object.");
                }
            }
            return new ObjectResult("object not updated");
        }

        // Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByID(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                bool update = await _businessLayer.DeleteByIdUsingAPI(id);
                if (update)
                {
                    return new ObjectResult("object deleted");
                }
                else
                {
                    return new ObjectResult("Api error - when trying to delete object.");
                }
            }
            return new ObjectResult("object not deleted");
        }

        #endregion


    }
}
