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
        //TODO Ska bytas ut mot businessLayer
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

            var fileItem = await _context.GetFileByIdUsingAPI( id ); //FileItems.SingleOrDefaultAsync( m => m.Id == id );

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

        //// POST: api/FileItems/5
        ////TODO l�gga till en location i return-objektet som man kan skicka fildatan till, kanske ska g�ras p� businesslayer?
        //[HttpPost( "{id}" )]
        //public async Task<IActionResult> PostDataChunk( [FromRoute] string id, [FromBody] DataChunk dataChunk ) {
        //    //TODO skicka fildatan till businesslayer och f� n�got kul tillbaka :P

        //    //if (!ModelState.IsValid)
        //    //{
        //    //    return BadRequest(ModelState);
        //    //}

        //    //_context.FileItems.Add(fileItem);
        //    //try
        //    //{
        //    //    await _context.SaveChangesAsync();
        //    //}
        //    //catch (DbUpdateException)
        //    //{
        //    //    if (FileItemExists(fileItem.Id))
        //    //    {
        //    //        return new StatusCodeResult(StatusCodes.Status409Conflict);
        //    //    }
        //    //    else
        //    //    {
        //    //        throw;
        //    //    }
        //    //}

        //    return CreatedAtAction( "GetFileItem", new { id = dataChunk.Id }, dataChunk );
        //}

        // DELETE: api/FileItems/5
        [HttpDelete( "{id}" )]
        public async Task<IActionResult> DeleteFileItem( [FromRoute] int id ) {
            if( !ModelState.IsValid ) {
                return BadRequest( ModelState );
            }

            //Kolla om filen finns först
            var fileItem = await _context.GetFileByIdUsingAPI( id ); // .FileItems.SingleOrDefaultAsync( m => m.Id == id );
            if( fileItem == null ) {
                return NotFound();
            }
            //Ta bort filen
            bool deleted = await _context.DeleteByIdUsingAPI( id );
            if( deleted )
                return Ok( fileItem );
            else
                return BadRequest( "Error while removing file" );
        }
    }
}
