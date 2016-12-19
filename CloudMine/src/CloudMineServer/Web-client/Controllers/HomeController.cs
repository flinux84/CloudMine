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
using Microsoft.AspNetCore.Identity;
using CloudMineServer.Models;
using CloudMineServer.Classes;
using CloudMineServer.Interface;
using CloudMineServer.API_server.Services;

namespace CloudMineServer.Controllers
{

    public class HomeController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private ICloudMineDbService _cloudMineDbService;

        private async Task<UserInfo> GetUserInfo(ApplicationUser user)
        {
            var userInfo = new UserInfo();

            userInfo.UserName = user.UserName;

            var fileItems = await _cloudMineDbService.GetAllFilesUsingAPI(user.Id);
            userInfo.NumberFiles = fileItems.ListFileItems.Count;

            userInfo.StorageSize = user.StorageSize;

            userInfo.UsedStorage = 0;
            fileItems.ListFileItems.ForEach(f => userInfo.UsedStorage += f.FileSize);

            return userInfo;
        }

        public HomeController(UserManager<ApplicationUser> userManager, ICloudMineDbService cloudMineDbService)
        {
            _userManager = userManager;
            _cloudMineDbService = cloudMineDbService;
        }


        public IActionResult Index()
        {

            return View();

        }


        [Authorize(Roles = "admin")]
        public async Task<ActionResult> AdminIndex()
        {

            var users = _userManager.Users.ToList();

            var allUserInfos = new List<UserInfo>();

            foreach (var user in users)
            {
                var userInfo = await GetUserInfo(user);
                allUserInfos.Add(userInfo);

            }

            return View(allUserInfos);

        }

        public async Task<ActionResult> Delete([FromRoute]string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByEmailAsync(id);

            if (user == null)
            {

                return NotFound();
            }

            return View(user);

        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var asdf = User.GetUserEmail();

            if (id == asdf)
            {

                return BadRequest($"Could not delete user: {id}");


            }

            var user = await _userManager.FindByEmailAsync(id);
            var result = await _userManager.DeleteAsync(user);
            if (result == IdentityResult.Success)
            { 
                return RedirectToAction("AdminIndex");
            }

            return BadRequest($"Could not delete user: {id}");

        }


        public async Task<IActionResult> Edit([FromRoute]string id)
        {

            var ApplUser = await _userManager.FindByEmailAsync(id);
            var user = await GetUserInfo(ApplUser);


            return View(user);

        }
        [HttpPost]
        [ActionName("Edit")]
        public async Task<IActionResult> EditUser(string id, UserInfo userInfo)
        {
            var user = await _userManager.FindByEmailAsync(id);
            var oldUserInfo = await GetUserInfo(user);

            if (oldUserInfo.UsedStorage > userInfo.StorageSize)
                return BadRequest("Can't shrink storage to less than your used storage!");

            user.StorageSize = userInfo.StorageSize;
            await _userManager.UpdateAsync(user);
            return View(userInfo);


        }
        public IActionResult Login()
        {

            return View();
        }

    }




    //public class YourCustomAuthorize : AuthorizeAttribute
    //{
    //    public override void OnAuthorization(AuthorizationContext filterContext)
    //    {
    //        // If they are authorized, handle accordingly
    //        if (this.AuthorizeCore(filterContext.HttpContext))
    //        {
    //            base.OnAuthorization(filterContext);
    //        }
    //        else
    //        {
    //            // Otherwise redirect to your specific authorized area
    //            filterContext.Result = new RedirectResult("~/YourController/Unauthorized");
    //        }
    //    }
    //}
    // Changes the unauth redirect to our own loginpage
    //Is it OK to add package to project?



}
