using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XInstallBotProfile.Migrations
{
    /// <inheritdoc />
    public partial class AddAllFlagsCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Banner",
                table: "user_statistic",
                newName: "IsDspInApp");

            migrationBuilder.AddColumn<bool>(
                name: "IsDsp",
                table: "user_statistic",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDspBanner",
                table: "user_statistic",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDsp",
                table: "user_statistic");

            migrationBuilder.DropColumn(
                name: "IsDspBanner",
                table: "user_statistic");

            migrationBuilder.RenameColumn(
                name: "IsDspInApp",
                table: "user_statistic",
                newName: "Banner");
        }
    }
}
