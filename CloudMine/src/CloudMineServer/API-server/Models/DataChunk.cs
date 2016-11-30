using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CloudMineServer.Models
{
    public class DataChunk
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        //public Guid DataChunkId { get; set; }

        public int PartIndex { get; set; }

        //ByteArray med datan
        public byte[] Data { get; set; }

        //Foreign key till FileItem för att matcha mot fil
        public int FileItemId { get; set; }

        public FileItem FileItem { get; set; }
    }
}
