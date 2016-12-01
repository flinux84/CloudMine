using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudMineServer.Models
{
    public class FileItemSet
    {
        public Guid UserId { get; set; }
        public List<FileItem> ListFileItems { get; set; }
    }
}
