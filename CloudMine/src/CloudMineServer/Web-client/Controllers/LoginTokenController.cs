using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudMineServer.Web_client.Controllers
{
    public class LoginTokenController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
