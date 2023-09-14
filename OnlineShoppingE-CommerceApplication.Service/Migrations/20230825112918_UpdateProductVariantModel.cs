using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShoppingE_CommerceApplication.Service.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductVariantModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "ProductVariant");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "ProductVariant",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
