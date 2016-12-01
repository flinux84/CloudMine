using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudMineServer.Models
{
    public class FileItem
    {
        public int Id { get; set; }

        public Guid Checksum { get; set; }

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

        // Användaren
        public Guid UserId { get; set; }
        // Går inte med olika contexts?
        //public ApplicationUser User { get; set; }
        
        //Fildatan för filen. Kan vara en, kan vara många.
        public virtual ICollection<DataChunk> DataChunks { get; set; }
    }
}
