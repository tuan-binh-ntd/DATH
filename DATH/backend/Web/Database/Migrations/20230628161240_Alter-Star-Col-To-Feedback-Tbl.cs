using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AlterStarColToFeedbackTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Star",
                table: "Feedback",
                type: "decimal(5,5)",
                precision: 5,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(1,1)",
                oldPrecision: 1,
                oldScale: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Star",
                table: "Feedback",
                type: "decimal(1,1)",
                precision: 1,
                scale: 1,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,5)",
                oldPrecision: 5,
                oldScale: 5);
        }
    }
}
