using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeBash.Web.Migrations
{
    public partial class MapFieldsToColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RelativeStrengthIndex",
                table: "Strategies",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SimpleMovingAverage",
                table: "Strategies",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelativeStrengthIndex",
                table: "Strategies");

            migrationBuilder.DropColumn(
                name: "SimpleMovingAverage",
                table: "Strategies");
        }
    }
}
