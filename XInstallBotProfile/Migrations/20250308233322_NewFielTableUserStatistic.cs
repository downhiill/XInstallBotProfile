using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace XInstallBotProfile.Migrations
{
    /// <inheritdoc />
    public partial class NewFielTableUserStatistic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_statistic_statistic_type_StatisticTypeId",
                table: "user_statistic");

            migrationBuilder.DropTable(
                name: "StatisticTypeUser");

            migrationBuilder.DropTable(
                name: "statistic_type");

            migrationBuilder.DropIndex(
                name: "IX_user_statistic_StatisticTypeId",
                table: "user_statistic");

            migrationBuilder.RenameColumn(
                name: "StatisticTypeId",
                table: "user_statistic",
                newName: "UserId");

            migrationBuilder.AddColumn<bool>(
                name: "Banner",
                table: "user_statistic",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Banner",
                table: "user_statistic");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "user_statistic",
                newName: "StatisticTypeId");

            migrationBuilder.CreateTable(
                name: "statistic_type",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_statistic_type", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatisticTypeUser",
                columns: table => new
                {
                    StatisticTypesId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticTypeUser", x => new { x.StatisticTypesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_StatisticTypeUser_statistic_type_StatisticTypesId",
                        column: x => x.StatisticTypesId,
                        principalTable: "statistic_type",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StatisticTypeUser_user_auth_info_UsersId",
                        column: x => x.UsersId,
                        principalTable: "user_auth_info",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_statistic_StatisticTypeId",
                table: "user_statistic",
                column: "StatisticTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StatisticTypeUser_UsersId",
                table: "StatisticTypeUser",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_user_statistic_statistic_type_StatisticTypeId",
                table: "user_statistic",
                column: "StatisticTypeId",
                principalTable: "statistic_type",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
