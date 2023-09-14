using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShoppingE_CommerceApplication.Service.Migrations
{
    /// <inheritdoc />
    public partial class productView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create view VProduct as SELECT
            Category.Id as CategoryId,Category.Name as CategoryName, Product.Id as ProductId,Product.Name,Product.Description,
              Stock.SellingPrice, 
            (Select Count(*) from ProductVariant as ProdVarCount where ProdVarCount.IsActive=1 and ProdVarCount.ProductId = Product.Id) As VariantCount,
          SUBSTRING(ProductVariant.Path,0,CHARINDEX('|',ProductVariant.Path,0)) as FilePath
          from Category
          inner join Product on Category.Id=Product.CategoryId
          inner join ProductVariant on Product.Id = ProductVariant.ProductId and ProductVariant.Id in 
        (SELECT  TOP (1) ProductVariant.Id AS Expr1  FROM  ProductVariant
        WHERE (ProductVariant.ProductId = Product.Id and ProductVariant.IsActive=1) )
        inner join Stock on ProductVariant.Id=Stock.ProductVariantId and Stock.Id in 
        (SELECT  TOP (1) Stock.Id AS Expr2
        FROM  Stock
        WHERE (Stock.ProductVariantId = ProductVariant.Id and Stock.IsActive=1) )
        where Category.IsActive=1 and Product.IsActive=1");
           
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            drop view VProduct;
            ");
        }
    }
}
