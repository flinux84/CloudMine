using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudMineServer.Models;

namespace CloudMineServer.API_server.Controllers
{
    [Produces("application/json")]
    [ApiVersion( "1.0" )]
    [Route( "api/v{version:apiVersion}/FileItems" )]
    public class FileItemsController : Controller
    {
        //TODO Ska bytas ut mot businessLayer
        private readonly CloudDbRepository _context;

        public FileItemsController(CloudDbRepository context)
        {
            _context = context;
        }

        // GET: api/FileItems
        [HttpGet]
        public IEnumerable<FileItem> GetFileItems()
        {
            return _context.FileItems;
        }

        // GET: api/FileItems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFileItem([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var fileItem = await _context.FileItems.SingleOrDefaultAsync(m => m.Id == id);

            if (fileItem == null)
            {
                return NotFound();
            }

            return Ok(fileItem);
        }

        // PUT: api/FileItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFileItem([FromRoute] string id, [FromBody] FileItem fileItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != fileItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(fileItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FileItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FileItems
        [HttpPost]
        public async Task<IActionResult> PostFileItem([FromBody] FileItem fileItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.FileItems.Add(fileItem);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FileItemExists(fileItem.Id))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFileItem", new { id = fileItem.Id }, fileItem);
        }

        // DELETE: api/FileItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileItem([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var fileItem = await _context.FileItems.SingleOrDefaultAsync(m => m.Id == id);
            if (fileItem == null)
            {
                return NotFound();
            }

            _context.FileItems.Remove(fileItem);
            await _context.SaveChangesAsync();

            return Ok(fileItem);
        }

        private bool FileItemExists(string id)
        {
            return _context.FileItems.Any(e => e.Id == id);
        }
    }
}
