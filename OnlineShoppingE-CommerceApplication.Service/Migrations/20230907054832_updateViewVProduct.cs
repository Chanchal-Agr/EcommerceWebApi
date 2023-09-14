using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShoppingE_CommerceApplication.Service.Migrations
{
    /// <inheritdoc />
    public partial class updateViewVProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"alter view [dbo].[VProduct] as select C.Id as CategoryId,C.Name as CategoryName, P.Id as ProductId,P.Name,P.Description,
S.SellingPrice,
(Select Count(*) from ProductVariant as ProdVarCount where ProdVarCount.IsActive=1 and ProdVarCount.ProductId = P.Id) As VariantCount,
SUBSTRING(PV.Path,0,CHARINDEX('|',PV.Path,0)) as FilePath 
from Product as P  
left outer join Category as C
on P.CategoryId = C.Id
left outer join ProductVariant as PV
on P.Id=PV.ProductId and PV.Id in 
(SELECT TOP (1) PVT.Id AS Expr1  
FROM  ProductVariant as PVT
WHERE (PVT.ProductId = P.Id and PVT.IsActive=1 and 
(select COUNT(Stock.Id) from Stock 
where(PVT.Id=Stock.ProductVariantId and Stock.IsActive=1)
) > 0))
left outer join Stock as S 
on PV.Id = S.ProductVariantId and S.Id in
(select top(1) Stock.Id from Stock 
where(PV.Id=Stock.ProductVariantId and Stock.IsActive=1 ))
where p.IsActive = 1 and c.IsActive = 1
;              
GO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"alter view [dbo].[VProduct] as SELECT
            Category.Id as CategoryId,Category.Name as CategoryName, Product.Id as ProductId,Product.Name,Product.Description,
              Stock.SellingPrice, 
(Select Count(*) from ProductVariant as ProdVarCount where ProdVarCount.IsActive=1 and ProdVarCount.ProductId = Product.Id) As VariantCount,
SUBSTRING(ProductVariant.Path,0,CHARINDEX('|',ProductVariant.Path,0)) as FilePath
from Category
 inner join Product on Category.Id=Product.CategoryId
inner join ProductVariant on Product.Id = ProductVariant.ProductId and ProductVariant.Id in 
(SELECT  TOP (1) ProductVariant.Id AS Expr1
 FROM  ProductVariant
WHERE (ProductVariant.ProductId = Product.Id and ProductVariant.IsActive=1) )
 inner join Stock on ProductVariant.Id=Stock.ProductVariantId and Stock.Id in 
(SELECT  TOP (1) Stock.Id AS Expr2
 FROM  Stock
WHERE (Stock.ProductVariantId = ProductVariant.Id and Stock.IsActive=1) )
 where Category.IsActive=1 and Product.IsActive=1
;              
GO");
        }
    }
}
