using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XInstallBotProfile.Migrations
{
    /// <inheritdoc />
    public partial class AddNewField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "user_auth_info",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "user_auth_info",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "user_auth_info",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "user_auth_info",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDsp",
                table: "user_auth_info",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDspBanner",
                table: "user_auth_info",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDspInApp",
                table: "user_auth_info",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "user_auth_info",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "user_auth_info");

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "user_auth_info");

            migrationBuilder.DropColumn(
                name: "IsDsp",
                table: "user_auth_info");

            migrationBuilder.DropColumn(
                name: "IsDspBanner",
                table: "user_auth_info");

            migrationBuilder.DropColumn(
                name: "IsDspInApp",
                table: "user_auth_info");

            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "user_auth_info");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "user_auth_info",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "user_auth_info",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
