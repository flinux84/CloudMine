using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CloudMineServer.Migrations
{
    public partial class GuidsAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataChunks_FileItems_FileItemId",
                table: "DataChunks");

            migrationBuilder.DropIndex(
                name: "IX_DataChunks_FileItemId",
                table: "DataChunks");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "FileItems",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "FileItems",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "FileItemId",
                table: "DataChunks",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "DataChunks",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<Guid>(
                name: "FileItemId1",
                table: "DataChunks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DataChunks_FileItemId1",
                table: "DataChunks",
                column: "FileItemId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DataChunks_FileItems_FileItemId1",
                table: "DataChunks",
                column: "FileItemId1",
                principalTable: "FileItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataChunks_FileItems_FileItemId1",
                table: "DataChunks");

            migrationBuilder.DropIndex(
                name: "IX_DataChunks_FileItemId1",
                table: "DataChunks");

            migrationBuilder.DropColumn(
                name: "FileItemId1",
                table: "DataChunks");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "FileItems",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "FileItems",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<string>(
                name: "FileItemId",
                table: "DataChunks",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "DataChunks",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.CreateIndex(
                name: "IX_DataChunks_FileItemId",
                table: "DataChunks",
                column: "FileItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_DataChunks_FileItems_FileItemId",
                table: "DataChunks",
                column: "FileItemId",
                principalTable: "FileItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
