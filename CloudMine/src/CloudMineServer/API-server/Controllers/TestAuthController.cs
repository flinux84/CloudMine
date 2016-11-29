using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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

    [Authorize]
    [Produces("application/json")]
    [Route("api/TestAuth")]
    public class TestAuthController : Controller
    {
        // GET: api/TestAuth
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
