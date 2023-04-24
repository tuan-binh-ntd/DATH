using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class ModifyShopIdCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Warehouse_Shop_ShopId",
                table: "Warehouse");

            migrationBuilder.DropIndex(
                name: "IX_Warehouse_ShopId",
                table: "Warehouse");

            migrationBuilder.AlterColumn<int>(
                name: "ShopId",
                table: "Warehouse",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_ShopId",
                table: "Warehouse",
                column: "ShopId",
                unique: true,
                filter: "[ShopId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouse_Shop_ShopId",
                table: "Warehouse",
                column: "ShopId",
                principalTable: "Shop",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Warehouse_Shop_ShopId",
                table: "Warehouse");

            migrationBuilder.DropIndex(
                name: "IX_Warehouse_ShopId",
                table: "Warehouse");

            migrationBuilder.AlterColumn<int>(
                name: "ShopId",
                table: "Warehouse",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_ShopId",
                table: "Warehouse",
                column: "ShopId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouse_Shop_ShopId",
                table: "Warehouse",
                column: "ShopId",
                principalTable: "Shop",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
