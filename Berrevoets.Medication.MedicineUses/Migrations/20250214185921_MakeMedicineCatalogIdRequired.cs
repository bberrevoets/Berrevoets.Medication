using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Berrevoets.Medication.MedicineUses.Migrations
{
    /// <inheritdoc />
    public partial class MakeMedicineCatalogIdRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MedicineCatalogId",
                table: "MedicineUses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MedicineCatalogId",
                table: "MedicineUses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
