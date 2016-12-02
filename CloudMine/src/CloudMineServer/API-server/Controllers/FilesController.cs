using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CloudMineServer.Data;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using CloudMineServer.Classes;
using System.Text;
using Microsoft.AspNetCore.Http;
using CloudMineServer.Services;

namespace CloudMineServer.Controllers
{   

    public class FilesController : Controller
    {

        private readonly ApplicationDbContext _context;
        private IHostingEnvironment _environment;

        public FilesController(ApplicationDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        //testing
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> GetAndSaveFiles()
        {
            for (int i = 0; i < Request.Form.Files.Count; i++)
            {
                
                var myFile = Request.Form.Files[i];                
                byte[] buffer = new byte[myFile.Length];
                myFile.OpenReadStream().Read(buffer, 0, 1024*1024);
                
                if (myFile != null && myFile.Length != 0)
                {
                    var PathForSaving = Path.Combine(_environment.WebRootPath, "images");
                    string path = Path.Combine(PathForSaving, myFile.FileName);
                    try
                    {
                        using (var fileStream = new FileStream(Path.Combine(PathForSaving, myFile.FileName), FileMode.Create))
                        {
                            
                            await myFile.CopyToAsync(fileStream);
                        }
                    }
                    catch
                    {
                        throw;
                    }

                }
            }
            return View();
        }


    }
}