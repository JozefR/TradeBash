using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeBash.Web.Migrations
{
    public partial class addStrategies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Strategies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SimpleMovingAverageParameter = table.Column<double>(nullable: false),
                    RelativeStrengthIndexParameter = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Strategies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stock",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(nullable: false),
                    Symbol = table.Column<string>(nullable: true),
                    Open = table.Column<double>(nullable: false),
                    Close = table.Column<double>(nullable: false),
                    Label = table.Column<string>(nullable: true),
                    SMA = table.Column<double>(nullable: true),
                    RSI = table.Column<double>(nullable: true),
                    StrategyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stock_Strategies_StrategyId",
                        column: x => x.StrategyId,
                        principalTable: "Strategies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stock_StrategyId",
                table: "Stock",
                column: "StrategyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stock");

            migrationBuilder.DropTable(
                name: "Strategies");
        }
    }
}
