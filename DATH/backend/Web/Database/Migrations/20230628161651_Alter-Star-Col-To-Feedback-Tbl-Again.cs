using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AlterStarColToFeedbackTblAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Star",
                table: "Feedback",
                type: "decimal(19,5)",
                precision: 19,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,5)",
                oldPrecision: 5,
                oldScale: 5);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Star",
                table: "Feedback",
                type: "decimal(5,5)",
                precision: 5,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,5)",
                oldPrecision: 19,
                oldScale: 5);
        }
    }
}
