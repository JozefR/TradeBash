using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeBash.Infrastructure.Migrations
{
    public partial class DontNeedDoubleForDrawdown : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DrawdownPercentage",
                table: "GeneratedOrders",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "DrawdownPercentage",
                table: "GeneratedOrders",
                type: "float",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
