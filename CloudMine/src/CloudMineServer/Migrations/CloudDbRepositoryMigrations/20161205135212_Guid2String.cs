using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CloudMineServer.Migrations.CloudDbRepositoryMigrations
{
    public partial class Guid2String : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Checksum",
                table: "FileItems",
                nullable: true,
                oldClrType: typeof(Guid));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Checksum",
                table: "FileItems",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
