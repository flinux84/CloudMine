using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CloudMineServer.Models;

namespace CloudMineServer.API_server.Controllers
{
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Users")]
    [ApiVersion("1.0")]
    public class UserApiController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserApiController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody]UserRegistrationModel userRegistration)
        {
            var user = new ApplicationUser { Email = userRegistration.Email, UserName = userRegistration.Email};
            user.StorageSize = 100000000;
            var result = await _userManager.CreateAsync(user, userRegistration.Password);
            if (result.Succeeded)
            {
                return CreatedAtAction("Get", new { id = user.Id }, new { UserName = user.UserName });
            }
            return BadRequest();
        }
    }

    public class UserRegistrationModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}