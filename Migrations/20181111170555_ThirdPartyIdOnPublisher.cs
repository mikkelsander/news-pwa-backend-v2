using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PWANews.Migrations
{
    public partial class ThirdPartyIdOnPublisher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "URL",
                table: "Publishers",
                newName: "Url");

            migrationBuilder.AddColumn<string>(
                name: "ThirdPartyId",
                table: "Publishers",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PublishedAt",
                table: "Articles",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThirdPartyId",
                table: "Publishers");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Publishers",
                newName: "URL");

            migrationBuilder.AlterColumn<string>(
                name: "PublishedAt",
                table: "Articles",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
