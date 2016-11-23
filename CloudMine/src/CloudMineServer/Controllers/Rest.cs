using CloudMineServer.Interface;
using CloudMineServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudMineServer.Controllers
{
    [Route("api/[Controller]")]
    [Authorize]
    public class Rest : Controller
    {
        #region Dependency Injection Constructor

        private readonly ICloudMineApi _businessLayer;

        public Rest(ICloudMineApi businessLayer)
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
            int num;
            if (int.TryParse(id, out num))
            {
                FileItem item = await _businessLayer.GetFileByIdUsingAPI(num);
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
            int num;
            if (int.TryParse(id, out num))
            {
                bool update = await _businessLayer.UpDateByIdUsingAPI(num, item);
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
            int num;
            if (int.TryParse(id, out num))
            {
                bool update = await _businessLayer.DeleteByIdUsingAPI(num);
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
