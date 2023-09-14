using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShoppingE_CommerceApplication.Service.Migrations
{
    /// <inheritdoc />
    public partial class updateStockTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Stock",
                newName: "StockToSale");

            migrationBuilder.AddColumn<int>(
                name: "AvailableQuantity",
                table: "Stock",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableQuantity",
                table: "Stock");

            migrationBuilder.RenameColumn(
                name: "StockToSale",
                table: "Stock",
                newName: "Quantity");
        }
    }
}
