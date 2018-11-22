using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PWANews.Migrations
{
    public partial class addedUsernamesInsteadOfEmails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "Users",
                newName: "IX_Users_Username");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Subscriptions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Publishers",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Subscriptions");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Username",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Publishers",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
