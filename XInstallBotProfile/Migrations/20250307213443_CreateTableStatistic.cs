using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace XInstallBotProfile.Migrations
{
    /// <inheritdoc />
    public partial class CreateTableStatistic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "user_auth_info",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "user_statistic",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Total = table.Column<long>(type: "bigint", nullable: false),
                    Ack = table.Column<long>(type: "bigint", nullable: false),
                    Win = table.Column<long>(type: "bigint", nullable: false),
                    ImpsCount = table.Column<long>(type: "bigint", nullable: false),
                    ShowRate = table.Column<decimal>(type: "numeric", nullable: false),
                    ClicksCount = table.Column<long>(type: "bigint", nullable: false),
                    Ctr = table.Column<long>(type: "bigint", nullable: false),
                    StartsCount = table.Column<long>(type: "bigint", nullable: false),
                    CompletesCount = table.Column<long>(type: "bigint", nullable: false),
                    Vtr = table.Column<long>(type: "bigint", nullable: false),
                    StatisticTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_statistic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_statistic_statistic_type_StatisticTypeId",
                        column: x => x.StatisticTypeId,
                        principalTable: "statistic_type",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_statistic_StatisticTypeId",
                table: "user_statistic",
                column: "StatisticTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_statistic");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "user_auth_info",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
