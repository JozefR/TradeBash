using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeBash.Infrastructure.Migrations
{
    public partial class AddedIndicatorValueForGeneratedOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CloseIndicators",
                table: "GeneratedOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenIndicators",
                table: "GeneratedOrders",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloseIndicators",
                table: "GeneratedOrders");

            migrationBuilder.DropColumn(
                name: "OpenIndicators",
                table: "GeneratedOrders");
        }
    }
}
