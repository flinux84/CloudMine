using CloudMineServer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudMineServer.Models
{
    public class CloudDbRepository : DbContext
    {
        public CloudDbRepository(DbContextOptions<CloudDbRepository> options) : base(options)
        {
        }

        public DbSet<FileItem> FileItems { get; set; }

        public DbSet<DataChunk> DataChunks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

    }
}
