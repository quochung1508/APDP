using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIMS.Migrations
{
    [Migration("20260101010101_AddUserExtendedProperties")]
    public partial class AddUserExtendedProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "aspnetusers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "aspnetusers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "aspnetusers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileId",
                table: "aspnetusers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "aspnetusers");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "aspnetusers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "aspnetusers");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "aspnetusers");
        }
    }
}
