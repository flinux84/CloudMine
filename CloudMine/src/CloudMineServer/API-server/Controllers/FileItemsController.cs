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
using CloudMineServer.API_server.Models;
using Microsoft.AspNetCore.Authorization;
using CloudMineServer.API_server.Services;
using System.IO;

namespace CloudMineServer.API_server.Controllers {
    [Produces( "application/json" )]
    [ApiVersion( "1.0" )]
    [Route( "api/v{version:apiVersion}/FileItems" )]
    [Authorize]
    public class FileItemsController : Controller {
        private readonly ICloudMineDbService _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUrlHelper _urlHelper;
        private const int maxPageSize = 20;

        public FileItemsController( ICloudMineDbService context, UserManager<ApplicationUser> userManager, IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor ) {
            _context = context;
            _userManager = userManager;
            _urlHelper = urlHelperFactory.GetUrlHelper( actionContextAccessor.ActionContext );
        }

        //GET: api/FileItems/checksum/id
        [HttpGet( "checksum/{id}" )]
        public async Task<bool> CheckCheckSum( string id ) {
            bool checksumExists = await _context.CheckChecksum( User.GetUserId(), id );
            return checksumExists;
        }

        // GET: api/FileItems
        [HttpGet( Name = "GetFileItems" )]
        public async Task<IEnumerable<FileItem>> GetFileItems( string filename = null, string description = null, string filetype = null, string sort = "id", string order = "asc", int pageNo = 1, int pageSize = maxPageSize ) {
            
            //Get all FileItems from a user
            FileItemSet fileItemSet = await _context.GetAllFilesUsingAPI( User.GetUserId() );
            IList<FileItem> fileItems = fileItemSet.ListFileItems;

            //reset pagesize if higher than maxPageSize
            if( pageSize > maxPageSize )
                pageSize = maxPageSize;

            //search string
            fileItems = fileItems.Where( x => ( filename == null || x.FileName.Contains( filename ) ) )
                .Where( x => ( description == null || x.Description.Contains( description ) ) )
                .Where( x => ( filetype == null || x.DataType.Contains( filetype ) ) ).ToList();

            //sort fileItems
            var sortedFileItems = fileItems.ApplySorting( sort, order );

            //metadata för paging
            int totalFileItems = sortedFileItems.Count;
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

            return sortedFileItems.Skip( ( pageNo - 1 ) * pageSize ).Take( pageSize );
        }

        // GET: api/FileItems/5
        [HttpGet( "{id:int}" )]
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
        [HttpPut( "{id:int}" )]
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
            fileItem.UserId = User.GetUserId();

            var metaDataCreated = await _context.InitCreateFileItem( fileItem );

            if( metaDataCreated ) {
                return CreatedAtAction( "GetFileItem", new { id = fileItem.Id }, fileItem );
            }

            return new StatusCodeResult( StatusCodes.Status400BadRequest );

        }

        // POST: api/FileItems/5
        [HttpPost( "{id:int}" )]
        public async Task<IActionResult> PostDataChunk( [FromRoute]int id, [FromForm]DataChunk dataChunk ) {
            
            // checksum
            bool checksumExists = await _context.CheckChecksum(User.GetUserId(), dataChunk.Checksum);
            if (checksumExists) {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            //Kolla dataChunk checksum om den redan finns, finns den så returnera ett error som javascriptet kan se och fortsätta på nästa chunk.
            //returna isåfall statuscode 409
            for (int i = 0; i < Request.Form.Files.Count; i++)
            {
                var file = Request.Form.Files[i];
                dataChunk.Data = StreamToArray(file.OpenReadStream());
            }
            
            //if( !ModelState.IsValid) {
            //    return BadRequest( ModelState );
            //}

            if( id != dataChunk.FileItemId )
                return BadRequest( "Id from route wasn't the same as fileItemId in DataChunk." );
            
            if( await _context.AddFileUsingAPI( dataChunk ) ) {
                return CreatedAtAction( "GetFileItem", new { id = dataChunk.Id }, dataChunk );
            } else
                return BadRequest( "Error adding datachunks." );
        }

        // DELETE: api/FileItems/5
        [HttpDelete( "{id:int}" )]
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

        private static byte[] StreamToArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

    }
}
