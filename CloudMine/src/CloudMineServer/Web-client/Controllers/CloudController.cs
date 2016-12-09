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

    public class CloudController : Controller
    {
        
        public CloudController()
        {

        }

        public IActionResult Index()
        {
            return Redirect("CloudMine.html");
            //return View();
        }

    }
}