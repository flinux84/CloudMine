using CloudMineServer.Interface;
using CloudMineServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace CloudMineServer.API_server.Controllers {
    [Produces("application/json")]
    [ApiVersion( "1.0" )]
    [Route( "api/v{version:apiVersion}/FileItems" )]
    public class FileItemsController : Controller
    {
        private readonly ICloudMineDbService _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUrlHelper _urlHelper;
        private const int maxPageSize = 20;

        public FileItemsController( ICloudMineDbService context, UserManager<ApplicationUser> userManager, IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor )
        {
            _context = context;
            _userManager = userManager;
            _urlHelper = urlHelperFactory.GetUrlHelper( actionContextAccessor.ActionContext );
        }

        // GET: api/FileItems
        [HttpGet( Name = "GetFileItems" )]
        public async Task<IEnumerable<FileItem>> GetFileItems(int pageNo = 1, int pageSize = maxPageSize) {
            //reset pagesize if higher than maxPageSize
            if( pageSize > maxPageSize )
                pageSize = maxPageSize;

            //metadata för paging
            FileItemSet fileItemSet = await _context.GetAllFilesUsingAPI( _userManager.GetUserId( User ) );
            IList<FileItem> fileItems = fileItemSet.ListFileItems;

            int totalFileItems = fileItems.Count;
            int totalPages = (int)Math.Ceiling( (double)totalFileItems / pageSize );
            
            //Previous link
            var prevPageLink = pageNo == 1 ? string.Empty : _urlHelper.Link( "GetFileItems",
                new {
                    pageNo = pageNo - 1,
                    pageSize = pageSize
                } );
            //Next link
            var nextPageLink = pageNo == totalPages ? string.Empty : _urlHelper.Link( "GetFileItems",
                new {
                    pageNo = pageNo + 1,
                    pageSize = pageSize
                } );

            //page header info
            var pageHeader = new {
                pageNo = pageNo,
                pageSize = pageSize,
                totalFileItems = totalFileItems,
                totalPages = totalPages,
                prevPageLink = prevPageLink,
                nextPageLink = nextPageLink
            };

            Response.Headers.Add( "X-PageInfo", JsonConvert.SerializeObject( pageHeader ) );

            return fileItems.Skip( ( pageNo - 1 ) * pageSize ).Take( pageSize );            
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
            fileItem.UserId = _userManager.GetUserId( HttpContext.User );

            var metaDataID = await _context.InitCreateFileItem( fileItem );

            if(metaDataID) {
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
