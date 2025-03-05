using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XInstallBotProfile.Migrations
{
    /// <inheritdoc />
    public partial class ConnectionUserAndStatisticType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "IX_StatisticTypeUser_UsersId",
                table: "StatisticTypeUser",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatisticTypeUser");
        }
    }
}
