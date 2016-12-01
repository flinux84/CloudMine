using CloudMineServer.Interface;
using CloudMineServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CloudMineServer.API_server.Controllers {
    [Produces("application/json")]
    [ApiVersion( "1.0" )]
    [Route( "api/v{version:apiVersion}/FileItems" )]
    public class FileItemsController : Controller
    {
        private readonly ICloudMineDbService _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FileItemsController( ICloudMineDbService context, UserManager<ApplicationUser> userManager )
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/FileItems
        [HttpGet]
        public async Task<FileItemSet> GetFileItems()
        {
            return await _context.GetAllFilesUsingAPI( _userManager.GetUserId( User ) );
        }

        // GET: api/FileItems/5
        [HttpGet( "{id}" )]
        public async Task<IActionResult> GetFileItem( [FromRoute] int id ) {
            if( !ModelState.IsValid ) {
                return BadRequest( ModelState );
            }

            var fileItem = await _context.GetFileByIdUsingAPI( id );

            if( fileItem == null ) {
                return NotFound();
            }

            return Ok( fileItem );
        }

        // PUT: api/FileItems/5
        [HttpPut( "{id}" )]
        public async Task<IActionResult> PutFileItem( [FromRoute] int id, [FromBody] FileItem fileItem ) {
            if( !ModelState.IsValid ) {
                return BadRequest( ModelState );
            }

            if( id != fileItem.Id ) {
                return BadRequest();
            }

            bool updated = await _context.UpDateByIdUsingAPI( id, fileItem );

            if( updated )
                return NoContent();
            else
                return BadRequest( "Data was not received properly." );
        }

        // POST: api/FileItems
        [HttpPost]
        public async Task<IActionResult> PostFileItem( [FromBody] FileItem fileItem ) {
            if( !ModelState.IsValid ) {
                return BadRequest( ModelState );
            }
            //Uppdatera userId på fileItem innan vi skickar den till business layer
            fileItem.UserId = _userManager.GetUserId( User );

            var metaDataID = await _context.InitCreateFileItem( fileItem );

            if(metaDataID != "" ) {
                return CreatedAtAction( "GetFileItem", new { id = fileItem.Id }, fileItem );    
            }

            return new StatusCodeResult( StatusCodes.Status400BadRequest );

        }

        // POST: api/FileItems/5
        //[HttpPost( "{id}" )]
        //public async Task<IActionResult> PostDataChunk( [FromRoute] int id, [FromBody] DataChunk dataChunk ) {

        //    if( !ModelState.IsValid ) {
        //        return BadRequest( ModelState );
        //    }

        //    //Hitta metadatan som chunksen tillhör
        //    var file = await _context.GetFileByIdUsingAPI( id );
        //    file.DataChunks.Add( dataChunk );
        //    dataChunk.FileItem = file;

        //    string chunksAdded = await _context.AddFileUsingAPI( dataChunk );

        //    if( chunksAdded == "Ok" )
        //        return CreatedAtAction( "GetFileItem", new { id = dataChunk.FileItemId }, dataChunk );
        //    else
        //        return BadRequest( "Error adding datachunks." );
        //}

        // DELETE: api/FileItems/5
        [HttpDelete( "{id}" )]
        public async Task<IActionResult> DeleteFileItem( [FromRoute] int id ) {
            if( !ModelState.IsValid ) {
                return BadRequest( ModelState );
            }

            //Kolla om filen finns först
            var fileItem = await _context.GetFileByIdUsingAPI( id );
            if( fileItem == null ) {
                return NotFound();
            }
            //Ta bort filen
            bool deleted = await _context.DeleteByIdUsingAPI( id );
            if( deleted )
                return Ok( fileItem ); //Returnerar objektet ifall man kanske skulle vilja ha en ångra sig funktionalitet?
            else
                return BadRequest( "Error while removing file" );
        }
    }
}
