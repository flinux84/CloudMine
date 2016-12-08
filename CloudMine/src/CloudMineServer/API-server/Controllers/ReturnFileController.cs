using CloudMineServer.API_server.Services;
using CloudMineServer.Interface;
using CloudMineServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudMineServer.API_server.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/GetFile")]

    public class Returnfile : Controller
    {
        private static HttpClient Client { get; } = new HttpClient();
        private ICloudMineDbService _context;
        public Returnfile(ICloudMineDbService context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public async Task<FileStreamResult> Get([FromRoute] int id)
        {
            var merge = new FileMerge();
            var userId = User.GetUserId();

            var stream = System.IO.File.OpenRead("D:\\Schoolprojects\\CloudMine\\CloudMine\\src\\CloudMineServer\\Temp\\StorageExplorer.exe");

            return new FileStreamResult(stream, new MediaTypeHeaderValue("application/octet-stream")) {
                FileDownloadName = "StorageExplorer.exe"
            };
            
        }


    }
}
