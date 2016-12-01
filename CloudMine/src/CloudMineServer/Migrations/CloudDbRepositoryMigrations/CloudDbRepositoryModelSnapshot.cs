using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using CloudMineServer.Models;

namespace CloudMineServer.Migrations.CloudDbRepositoryMigrations
{
    [DbContext(typeof(CloudDbRepository))]
    partial class CloudDbRepositoryModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CloudMineServer.Models.DataChunk", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("Data");

                    b.Property<int>("FileItemId");

                    b.Property<string>("PartName");

                    b.HasKey("Id");

                    b.HasIndex("FileItemId");

                    b.ToTable("DataChunks");
                });

            modelBuilder.Entity("CloudMineServer.Models.FileItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("Checksum");

                    b.Property<string>("DataType");

                    b.Property<string>("Description");

                    b.Property<string>("FileName");

                    b.Property<int>("FileSize");

                    b.Property<bool>("Private");

                    b.Property<DateTime>("Uploaded");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("FileItems");
                });

            modelBuilder.Entity("CloudMineServer.Models.DataChunk", b =>
                {
                    b.HasOne("CloudMineServer.Models.FileItem", "FileItem")
                        .WithMany("DataChunks")
                        .HasForeignKey("FileItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
