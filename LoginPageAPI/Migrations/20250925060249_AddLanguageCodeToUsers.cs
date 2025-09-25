﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginPageAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguageCodeToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LanguageCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LanguageCode",
                table: "AspNetUsers");
        }
    }
}
