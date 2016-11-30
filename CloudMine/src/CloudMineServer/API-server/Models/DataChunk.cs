using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CloudMineServer.Models
{
    public class DataChunk
    {
        [Key]

        //GUID här med?
        public string Id { get; set; }

        //Kommer från javascriptet där namnet på chunken får "part 1 av 5" t.ex
        public string PartIndex { get; set; }

        //ByteArray med datan
        public byte[] Data { get; set; }

        //Foreign key till FileItem för att matcha mot fil
        public string FileItemId { get; set; }

        public FileItem FileItem { get; set; }
    }
}
