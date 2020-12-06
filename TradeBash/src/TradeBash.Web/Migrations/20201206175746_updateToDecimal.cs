using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeBash.Web.Migrations
{
    public partial class updateToDecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SimpleMovingAverageParameter",
                table: "Strategies",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "RelativeStrengthIndexParameter",
                table: "Strategies",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "SMA",
                table: "Stock",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "RSI",
                table: "Stock",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Open",
                table: "Stock",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "Close",
                table: "Stock",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "SimpleMovingAverageParameter",
                table: "Strategies",
                type: "float",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<double>(
                name: "RelativeStrengthIndexParameter",
                table: "Strategies",
                type: "float",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<double>(
                name: "SMA",
                table: "Stock",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "RSI",
                table: "Stock",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Open",
                table: "Stock",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Close",
                table: "Stock",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
