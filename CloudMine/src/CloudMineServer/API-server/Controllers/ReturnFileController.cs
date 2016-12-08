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


        //Denna fungerar enligt "gamla metoden". Filen skapas på servern och returneras till klienten.
        //dock ligger filen kvar på servern så den tas inte bort, kanske går att lägga till funktion för det.
        [HttpGet("{id:int}")]
        public async Task<FileStreamResult> Get([FromRoute] int id)
        {
            var merge = new FileMerge();
            var userId = User.GetUserId();

            var file = await _context.GetSpecifikFileItemAndDataChunk(id, userId);

            var merger = new FileMerge();
            var uri = merger.MakeFileOnServer(file);
            
            var stream = System.IO.File.OpenRead(uri.AbsolutePath);

            return new FileStreamResult(stream, new MediaTypeHeaderValue("application/octet-stream")) {
                FileDownloadName = file.FileName
            };
            
        }

        //Martins förslag bygger på att man använder en ny returtyp som ärver FileResult.
        //Denna har jag lagt i services och heter FileCallbackResult och ska kunna hantera chunks av streams.
        //Så då skulle man isåfall implementera en annan Get-metod som använder denna och vi skippar isåfall att använda FileMerge().
        //Länk att läsa på detta: http://blog.stephencleary.com/2016/11/streaming-zip-on-aspnet-core.html
        // i länken så skapar han upp en zip-fil "on the fly", men vi får anpassa den isåfall.

    }
}
