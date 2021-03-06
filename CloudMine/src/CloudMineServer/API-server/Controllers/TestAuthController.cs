using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CloudMineServer.Models;
using System.IdentityModel.Tokens.Jwt;
using CloudMineServer.API_server.Services;
using CloudMineServer.API_server.Models;

namespace CloudMineServer.Controllers
{
    /// <summary>
    /// Temporary api controller for testing
    /// Should force you to log in
    /// To log in send this to http://serverurl/token
    ///
    /// POST /token
    /// Content-Type: application/x-www-form-urlencoded
    /// username = TEST & password = TEST123
    /// 
    /// You will get this response
    /// 200 OK
    /// Content-Type: application/json
    /// {
    ///   "access_token": "eyJhb...",
    ///   "expires_in": 300
    /// }
    /// 
    /// Use a header with 
    /// key: Authorization
    /// value: Bearer eyJhbGciO....
    /// </summary> 

    
    [Produces("application/json")]
    [Route("api/TestAuth")]
    public class TestAuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public TestAuthController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        // GET: api/TestAuth
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            List<DataChunk> dcs = new List<DataChunk>
            {
                new DataChunk {PartName= "aha.jpg.part_8.10"},
                new DataChunk {PartName= "aha.jpg.part_2.10" },
                new DataChunk {PartName= "aha.jpg.part_4.10" },
                new DataChunk {PartName= "aha.jpg.part_9.10" },
                new DataChunk {PartName= "aha.jpg.part_10.10" },
                new DataChunk {PartName= "aha.jpg.part_7.10" },
                new DataChunk {PartName= "aha.jpg.part_6.10" },
                new DataChunk {PartName= "aha.jpg.part_5.10" },
                new DataChunk {PartName= "aha.jpg.part_1.10" },
                new DataChunk {PartName= "aha.jpg.part_3.10" }
            };
            dcs.Sort(new DataChunkPartNameComparer());

            var userId = User.GetUserId();
            var userEmail = User.GetUserEmail();

            return new string[] { userEmail, userId };
        }


    }
}
