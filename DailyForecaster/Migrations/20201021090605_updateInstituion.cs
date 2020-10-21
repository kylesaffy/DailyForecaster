using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class updateInstituion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "NumberOfProducts",
                table: "ItemisedProducts",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<long>(
                name: "ProviderId",
                table: "Institution",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "Institution");

            migrationBuilder.AlterColumn<int>(
                name: "NumberOfProducts",
                table: "ItemisedProducts",
                type: "int",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
