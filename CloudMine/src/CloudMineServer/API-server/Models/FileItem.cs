﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudMineServer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class FileItem
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public byte[] FileData { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public bool Private { get; set; }
        public string DataType { get; set; }
        public int FileSize { get; set; }
        public int FileChunkId { get; set; }
        public int FileChunkIndex { get; set; }
    }
}