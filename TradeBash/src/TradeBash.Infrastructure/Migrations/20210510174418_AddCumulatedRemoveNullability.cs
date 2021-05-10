using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeBash.Infrastructure.Migrations
{
    public partial class AddCumulatedRemoveNullability : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Budget",
                table: "Strategies",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CumulatedCapital",
                table: "GeneratedOrders",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CumulatedCapital",
                table: "GeneratedOrders");

            migrationBuilder.AlterColumn<int>(
                name: "Budget",
                table: "Strategies",
                type: "int",
                nullable: true,
                oldClrType: typeof(double));
        }
    }
}
