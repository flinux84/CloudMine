using CloudMineServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CloudMineServer.API_server.Models
{
    public static class FileItemSortHelper {
        public static IList<FileItem> ApplySorting( this IList<FileItem> allFileItems, string sortby, string order ) {
            IList<FileItem> sortedFileItems;

            if( allFileItems == null ) {
                throw new ArgumentNullException( "No FileItems in IList." );
            }

            if( order.ToLowerInvariant() == "desc" ) {
                sortedFileItems = allFileItems.OrderByDescending( x => x.GetType().GetProperty( sortby, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase ).GetValue( x, null ) ).ToList();
            } else if( order.ToLowerInvariant() == "asc" ) {
                sortedFileItems = allFileItems.OrderBy( x => x.GetType().GetProperty( sortby, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase ).GetValue( x, null ) ).ToList();
            } else {
                sortedFileItems = allFileItems;
            }

            return sortedFileItems.ToList();
        }
    }
}
