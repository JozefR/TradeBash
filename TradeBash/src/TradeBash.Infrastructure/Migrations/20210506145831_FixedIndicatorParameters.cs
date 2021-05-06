using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeBash.Infrastructure.Migrations
{
    public partial class FixedIndicatorParameters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelativeStrengthIndex",
                table: "Strategies");

            migrationBuilder.DropColumn(
                name: "SimpleMovingAverage",
                table: "Strategies");

            migrationBuilder.AddColumn<int>(
                name: "RSI",
                table: "Strategies",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SMALong",
                table: "Strategies",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SMAShort",
                table: "Strategies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RSI",
                table: "Strategies");

            migrationBuilder.DropColumn(
                name: "SMALong",
                table: "Strategies");

            migrationBuilder.DropColumn(
                name: "SMAShort",
                table: "Strategies");

            migrationBuilder.AddColumn<int>(
                name: "RelativeStrengthIndex",
                table: "Strategies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SimpleMovingAverage",
                table: "Strategies",
                type: "int",
                nullable: true);
        }
    }
}
