using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using CloudMineServer.API_server.Controllers;
using System.Security.Principal;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace CloudMineServer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult>AdminIndex()
        {
            

            var client = new HttpClient();
                       

            var response = await client.GetAsync("http://localhost:2015/api/v1.0/Users/");

            var jsonResult = await response.Content.ReadAsStringAsync();


            var users = JsonConvert.DeserializeObject<IEnumerable<UserInfo>>(jsonResult);
            

            return View(users);
            
           
        }



        //public async Task<ActionResult> Delete(string username)
        //{

        //    if (username == null)
        //    {

        //        return NotFound();
        //    }
        //    var client = new HttpClient();
        //    var response = await client.GetAsync("http://localhost:2015/api/v1.0/Users/");

        //    var jsonResult = await response.Content.ReadAsStringAsync();


        //    var users = JsonConvert.DeserializeObject<IEnumerable<UserInfo>>(jsonResult);

        //    var userToDelete = users.Where(x => x.UserName == username);


        //    return View();


        //}



    }
}
