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
        public Guid Id { get; set; }

        //klienten skickar namn på part, plocka ut index och sätt ("part 2.5" t.ex blir index 2 (av totalt 5))
        public int PartIndex { get; set; }

        //ByteArray med datan
        public byte[] Data { get; set; }

        //Foreign key till FileItem för att matcha mot fil
        public Guid FileItemId { get; set; }

        public FileItem FileItem { get; set; }
    }
}
