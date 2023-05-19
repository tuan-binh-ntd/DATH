using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecificationIdToPhotoTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SpecificationId",
                table: "Photo",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Photo_SpecificationId",
                table: "Photo",
                column: "SpecificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photo_Specification_SpecificationId",
                table: "Photo",
                column: "SpecificationId",
                principalTable: "Specification",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photo_Specification_SpecificationId",
                table: "Photo");

            migrationBuilder.DropIndex(
                name: "IX_Photo_SpecificationId",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "SpecificationId",
                table: "Photo");
        }
    }
}
