using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using CloudMineServer.Models;

namespace CloudMineServer.Migrations
{
    [DbContext(typeof(CloudDbRepository))]
    [Migration("20161130140925_GuidsAgain")]
    partial class GuidsAgain
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CloudMineServer.Models.DataChunk", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<byte[]>("Data");

                    b.Property<int>("FileItemId");

                    b.Property<Guid?>("FileItemId1");

                    b.Property<int>("PartIndex");

                    b.HasKey("Id");

                    b.HasIndex("FileItemId1");

                    b.ToTable("DataChunks");
                });

            modelBuilder.Entity("CloudMineServer.Models.FileItem", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<string>("DataType");

                    b.Property<string>("Description");

                    b.Property<string>("FileName");

                    b.Property<int>("FileSize");

                    b.Property<bool>("Private");

                    b.Property<DateTime>("Uploaded");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.ToTable("FileItems");
                });

            modelBuilder.Entity("CloudMineServer.Models.DataChunk", b =>
                {
                    b.HasOne("CloudMineServer.Models.FileItem", "FileItem")
                        .WithMany("DataChunk")
                        .HasForeignKey("FileItemId1");
                });
        }
    }
}
