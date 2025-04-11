using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace XInstallBotProfile.Migrations
{
    /// <inheritdoc />
    public partial class CreateTableXInstallAppStatistic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsXInstallApp",
                table: "user_auth_info",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "xinstallapp_user_stat",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Total = table.Column<long>(type: "bigint", nullable: false),
                    AppLink = table.Column<string>(type: "text", nullable: false),
                    AppName = table.Column<string>(type: "text", nullable: false),
                    Region = table.Column<string>(type: "text", nullable: false),
                    KeywordsSerialized = table.Column<string>(type: "text", nullable: true),
                    TotalInstall = table.Column<long>(type: "bigint", nullable: false),
                    Complited = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_xinstallapp_user_stat", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "xinstallapp_user_stat");

            migrationBuilder.DropColumn(
                name: "IsXInstallApp",
                table: "user_auth_info");
        }
    }
}
