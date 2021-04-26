using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeBash.Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Strategies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    RelativeStrengthIndex = table.Column<int>(nullable: true),
                    SimpleMovingAverage = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Strategies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StocksHistory",
                columns: table => new
                {
                    StockId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Close = table.Column<double>(nullable: false),
                    High = table.Column<double>(nullable: false),
                    Low = table.Column<double>(nullable: false),
                    Open = table.Column<double>(nullable: false),
                    Volume = table.Column<double>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    ChangeOverTime = table.Column<double>(nullable: false),
                    MarketChangeOverTime = table.Column<double>(nullable: false),
                    UOpen = table.Column<double>(nullable: false),
                    UClose = table.Column<double>(nullable: false),
                    UHigh = table.Column<double>(nullable: false),
                    ULow = table.Column<double>(nullable: false),
                    UVolume = table.Column<double>(nullable: false),
                    FOpen = table.Column<double>(nullable: false),
                    FClose = table.Column<double>(nullable: false),
                    FHigh = table.Column<double>(nullable: false),
                    FLow = table.Column<double>(nullable: false),
                    FVolume = table.Column<double>(nullable: false),
                    Label = table.Column<string>(nullable: true),
                    Change = table.Column<double>(nullable: false),
                    ChangePercent = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StocksHistory", x => new { x.StockId, x.Id });
                    table.ForeignKey(
                        name: "FK_StocksHistory_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneratedOrder",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(nullable: true),
                    Ticker = table.Column<string>(nullable: true),
                    OpenPrice = table.Column<double>(nullable: false),
                    ClosePrice = table.Column<double>(nullable: true),
                    OpenDate = table.Column<DateTime>(nullable: false),
                    CloseDate = table.Column<DateTime>(nullable: true),
                    ProfitLoss = table.Column<double>(nullable: true),
                    Position = table.Column<int>(nullable: false),
                    StrategyId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneratedOrder_Strategies_StrategyId",
                        column: x => x.StrategyId,
                        principalTable: "Strategies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StrategyStock",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(nullable: false),
                    Label = table.Column<string>(nullable: false),
                    StrategyId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StrategyStock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StrategyStock_Strategies_StrategyId",
                        column: x => x.StrategyId,
                        principalTable: "Strategies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CalculatedStock",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(nullable: false),
                    Open = table.Column<double>(nullable: false),
                    Close = table.Column<double>(nullable: false),
                    SMA = table.Column<double>(nullable: true),
                    RSI = table.Column<double>(nullable: true),
                    StrategySignal = table.Column<string>(nullable: true),
                    ProfitLoss = table.Column<double>(nullable: true),
                    StrategyStockId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculatedStock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalculatedStock_StrategyStock_StrategyStockId",
                        column: x => x.StrategyStockId,
                        principalTable: "StrategyStock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalculatedStock_StrategyStockId",
                table: "CalculatedStock",
                column: "StrategyStockId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedOrder_StrategyId",
                table: "GeneratedOrder",
                column: "StrategyId");

            migrationBuilder.CreateIndex(
                name: "IX_StrategyStock_StrategyId",
                table: "StrategyStock",
                column: "StrategyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalculatedStock");

            migrationBuilder.DropTable(
                name: "GeneratedOrder");

            migrationBuilder.DropTable(
                name: "StocksHistory");

            migrationBuilder.DropTable(
                name: "StrategyStock");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "Strategies");
        }
    }
}
