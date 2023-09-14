using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShoppingE_CommerceApplication.Service.Migrations
{
    /// <inheritdoc />
    public partial class removePriceFromCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Cart");

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Cart",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
