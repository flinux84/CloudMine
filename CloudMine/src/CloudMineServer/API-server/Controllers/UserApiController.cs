using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CloudMineServer.Models;
using CloudMineServer.Interface;
using Microsoft.AspNetCore.Authorization;

namespace CloudMineServer.API_server.Controllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Users")]
    public class UserApiController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICloudMineDbService _cloudMineDbService;

        public UserApiController(UserManager<ApplicationUser> userManager, ICloudMineDbService cloudMineDbService)
        {
            _userManager = userManager;
            _cloudMineDbService = cloudMineDbService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody]UserRegistrationModel userRegistration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser { Email = userRegistration.Email, UserName = userRegistration.Email};
            user.StorageSize = 100000000;

            var result = await _userManager.CreateAsync(user, userRegistration.Password);
            if (result.Succeeded)
            {
                return CreatedAtAction("Get", new { userName = user.UserName }, new UserInfo { UserName = user.UserName, StorageSize = user.StorageSize });
            }

            return BadRequest();
        }

        [HttpGet("{userEmail}")]
        [Authorize]
        public async Task<UserInfo> GetUserInfo([FromRoute]string userEmail)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return await GetUserInfo(user);
        }

        // TODO: Should probably be available for admin role only
        //[Authorize]
        [HttpGet]
        public async Task<List<UserInfo>> GetUsersInfos()
        {
            var users = _userManager.Users.ToList();
            var allUserInfos = new List<UserInfo>();

            //users.ForEach(async u => allUserInfos.Add(await GetUserInfo(u)));
            foreach (var user in users)
            {
                var userInfo = await GetUserInfo(user);
                allUserInfos.Add(userInfo);
            }
            return allUserInfos;
        }

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
    }

    public class UserRegistrationModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class UserInfo
    {
        public string UserName { get; set; }
        public int StorageSize { get; set; }
        public int UsedStorage { get; set; }
        public int NumberFiles { get; set; }
    }
}