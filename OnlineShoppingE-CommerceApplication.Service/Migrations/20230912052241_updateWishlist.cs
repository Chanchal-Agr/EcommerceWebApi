using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShoppingE_CommerceApplication.Service.Migrations
{
    /// <inheritdoc />
    public partial class updateWishlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Wishlist");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Wishlist",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
