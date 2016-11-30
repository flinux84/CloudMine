﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CloudMineServer.Migrations
{
    public partial class cloud : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileItems",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DataType = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    FileSize = table.Column<int>(nullable: false),
                    Private = table.Column<bool>(nullable: false),
                    Uploaded = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataChunks",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Data = table.Column<byte[]>(nullable: true),
                    FileItemId = table.Column<string>(nullable: true),
                    PartIndex = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataChunks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataChunks_FileItems_FileItemId",
                        column: x => x.FileItemId,
                        principalTable: "FileItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataChunks_FileItemId",
                table: "DataChunks",
                column: "FileItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataChunks");

            migrationBuilder.DropTable(
                name: "FileItems");
        }
    }
}
