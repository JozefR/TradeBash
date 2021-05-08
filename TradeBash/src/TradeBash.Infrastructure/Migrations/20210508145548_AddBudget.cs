using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeBash.Infrastructure.Migrations
{
    public partial class AddBudget : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Budget",
                table: "Strategies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Strategies");
        }
    }
}
