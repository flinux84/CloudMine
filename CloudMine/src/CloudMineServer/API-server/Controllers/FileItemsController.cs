using CloudMineServer.Interface;
using CloudMineServer.Models;
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
        private readonly UserManager<ApplicationUser> userManager;

        public FileItemsController( ICloudMineDbService context, UserManager<ApplicationUser> _userManager )
        {
            _context = context;
            userManager = _userManager;
        }

        // GET: api/FileItems
        [HttpGet]
        public async Task<FileItemSet> GetFileItems()
        {
            //a345204b - c91a - 42e4 - 87a0 - 03eb585090b1
            Guid g = new Guid( userManager.GetUserId( User ) );
            return await _context.GetAllFilesUsingAPI( g );
        }

        //// GET: api/FileItems/5
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetFileItem([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var fileItem = await _context.FileItems.SingleOrDefaultAsync(m => m.Id == id);

        //    if (fileItem == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(fileItem);
        //}

        //// PUT: api/FileItems/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutFileItem([FromRoute] int id, [FromBody] FileItem fileItem)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != fileItem.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(fileItem).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!FileItemExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}
        //// POST: api/FileItems
        //[HttpPost]
        //public async Task<IActionResult> PostFileItem( [FromBody] FileItem fileItem ) {
        //    if( !ModelState.IsValid ) {
        //        return BadRequest( ModelState );
        //    }

        //    _context.FileItems.Add( fileItem );
        //    try {
        //        await _context.SaveChangesAsync();
        //    } catch( DbUpdateException ) {
        //        if( FileItemExists( fileItem.Id ) ) {
        //            return new StatusCodeResult( StatusCodes.Status409Conflict );
        //        } else {
        //            throw;
        //        }
        //    }

        //    return CreatedAtAction( "GetFileItem", new { id = fileItem.Id }, fileItem );
        //}

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

        //// DELETE: api/FileItems/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteFileItem([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var fileItem = await _context.FileItems.SingleOrDefaultAsync(m => m.Id == id);
        //    if (fileItem == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.FileItems.Remove(fileItem);
        //    await _context.SaveChangesAsync();

        //    return Ok(fileItem);
        //}

        //private bool FileItemExists(int id)
        //{
        //    return _context.FileItems.Any(e => e.Id == id);
        //}
    }
}
