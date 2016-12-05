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
        public int Id { get; set; }

        public string PartName { get; set; }

        public string Checksum { get; set; }

        //ByteArray med datan
        public byte[] Data { get; set; }

        //Foreign key till FileItem för att matcha mot fil

        public int FileItemId { get; set; }

        public FileItem FileItem { get; set; }
    }
}
