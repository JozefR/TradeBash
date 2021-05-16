using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeBash.Infrastructure.Migrations
{
    public partial class RemovedIntradayDrawdown : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DrawdownFromLowPricePercentage",
                table: "GeneratedOrders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DrawdownFromLowPricePercentage",
                table: "GeneratedOrders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
