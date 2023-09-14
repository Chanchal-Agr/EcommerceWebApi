using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShoppingE_CommerceApplication.Service.Migrations
{
    /// <inheritdoc />
    public partial class updateWishlistTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wishlist_ProductVariant_ProductVariantId",
                table: "Wishlist");

            migrationBuilder.RenameColumn(
                name: "ProductVariantId",
                table: "Wishlist",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Wishlist_ProductVariantId",
                table: "Wishlist",
                newName: "IX_Wishlist_ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlist_Product_ProductId",
                table: "Wishlist",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wishlist_Product_ProductId",
                table: "Wishlist");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Wishlist",
                newName: "ProductVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_Wishlist_ProductId",
                table: "Wishlist",
                newName: "IX_Wishlist_ProductVariantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlist_ProductVariant_ProductVariantId",
                table: "Wishlist",
                column: "ProductVariantId",
                principalTable: "ProductVariant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
