using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeBash.Infrastructure.Migrations
{
    public partial class AddLowPriceToEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DrawdownFromLowPricePercentage",
                table: "GeneratedOrders",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Low",
                table: "CalculatedStocksHistory",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DrawdownFromLowPricePercentage",
                table: "GeneratedOrders");

            migrationBuilder.DropColumn(
                name: "Low",
                table: "CalculatedStocksHistory");
        }
    }
}
