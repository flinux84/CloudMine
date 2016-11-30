using System;
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
        // Generera GUID saltat med UserId och FileName, behöver då vara string.
        public int Id { get; set; }

        public Guid FileItemId { get; set; }

        // Användarens GUID
        public Guid UserId { get; set; }

        // Filnamnet
        public string FileName { get; set; }

        //Valfri beskrivning som kan läggas till efter upload
        public string Description { get; set; }

        //Tidstämpel då filen laddades upp
        public DateTime Uploaded { get; set; }

        //Privat eller delad?
        public bool Private { get; set; }

        //Datatyp, plockas ut från filnamnet?
        public string DataType { get; set; }

        //Filstorlek på hela filen
        public int FileSize { get; set; }

        //Fildatan för filen. Kan vara en, kan vara många.
        public virtual ICollection<DataChunk> DataChunk { get; set; }
    }
}
