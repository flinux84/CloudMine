using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CloudMineServer.Data;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using CloudMineServer.Classes;

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
                    Merge Items = new Merge();
                    Items.MergeFile(path);
                }
            }
            return View();
        }
    }
}