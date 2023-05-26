using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentIdColToOrderTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_PaymentId",
                table: "Order",
                column: "PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Payment_PaymentId",
                table: "Order",
                column: "PaymentId",
                principalTable: "Payment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Payment_PaymentId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_PaymentId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Order");
        }
    }
}
