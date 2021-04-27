using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeBash.Infrastructure.Migrations
{
    public partial class AddSymbol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfitLoss",
                table: "CalculatedStock");

            migrationBuilder.DropColumn(
                name: "StrategySignal",
                table: "CalculatedStock");

            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "CalculatedStock",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "CalculatedStock");

            migrationBuilder.AddColumn<double>(
                name: "ProfitLoss",
                table: "CalculatedStock",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StrategySignal",
                table: "CalculatedStock",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
