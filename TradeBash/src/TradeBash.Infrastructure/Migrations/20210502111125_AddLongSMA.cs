using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeBash.Infrastructure.Migrations
{
    public partial class AddLongSMA : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SMA",
                table: "CalculatedStock");

            migrationBuilder.AddColumn<double>(
                name: "SMALong",
                table: "CalculatedStock",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SMAShort",
                table: "CalculatedStock",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SMALong",
                table: "CalculatedStock");

            migrationBuilder.DropColumn(
                name: "SMAShort",
                table: "CalculatedStock");

            migrationBuilder.AddColumn<double>(
                name: "SMA",
                table: "CalculatedStock",
                type: "float",
                nullable: true);
        }
    }
}
