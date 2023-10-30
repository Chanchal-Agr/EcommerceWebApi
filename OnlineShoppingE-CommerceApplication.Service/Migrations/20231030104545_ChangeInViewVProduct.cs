using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShoppingE_CommerceApplication.Service.Migrations
{
    /// <inheritdoc />
    public partial class ChangeInViewVProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"alter view [dbo].[VProduct] as SELECT   C.Id AS CategoryId, C.Name AS CategoryName, P.Id AS ProductId, P.Name, P.Description, S.SellingPrice,  
             (SELECT  COUNT(*) AS Expr1
             FROM  dbo.ProductVariant AS ProdVarCount
             WHERE (IsActive = 1) AND (ProductId = P.Id)) AS VariantCount,  CASE
             WHEN Path NOT LIKE '%|%' THEN Path
             ELSE SUBSTRING(Path, 0, CHARINDEX('|',Path))
             END AS FilePath
             FROM  dbo.Product AS P 
             LEFT OUTER JOIN
             dbo.Category AS C ON P.CategoryId = C.Id 
             LEFT OUTER JOIN
             dbo.ProductVariant AS PV ON P.Id = PV.ProductId AND PV.Id IN
             (SELECT  TOP (1) Id AS Expr1
             FROM  dbo.ProductVariant AS PVT
             WHERE (ProductId = P.Id) AND (IsActive = 1) AND
             ((SELECT COUNT(Id) AS Expr1
             FROM   dbo.Stock
             WHERE  (PVT.Id = ProductVariantId) AND (IsActive = 1)) >= 0)) 
             LEFT OUTER JOIN
             dbo.Stock AS S ON PV.Id = S.ProductVariantId AND S.Id IN
             (SELECT TOP (1) Id
             FROM  dbo.Stock
             WHERE  (PV.Id = ProductVariantId) AND (IsActive = 1))
             WHERE  (P.IsActive = 1) AND (C.IsActive = 1);
             GO");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"alter view [dbo].[VProduct] as SELECT   C.Id AS CategoryId, C.Name AS CategoryName, P.Id AS ProductId, P.Name, P.Description, S.SellingPrice,  
             (SELECT  COUNT(*) AS Expr1
             FROM  dbo.ProductVariant AS ProdVarCount
             WHERE (IsActive = 1) AND (ProductId = P.Id)) AS VariantCount,  CASE
             WHEN Path NOT LIKE '%|%' THEN Path
             ELSE SUBSTRING(Path, 0, CHARINDEX('|',Path))
             END AS FilePath
             FROM  dbo.Product AS P 
             LEFT OUTER JOIN
             dbo.Category AS C ON P.CategoryId = C.Id 
             LEFT OUTER JOIN
             dbo.ProductVariant AS PV ON P.Id = PV.ProductId AND PV.Id IN
             (SELECT  TOP (1) Id AS Expr1
             FROM  dbo.ProductVariant AS PVT
             WHERE (ProductId = P.Id) AND (IsActive = 1) AND
             ((SELECT COUNT(Id) AS Expr1
             FROM   dbo.Stock
             WHERE  (PVT.Id = ProductVariantId) AND (IsActive = 1)) >= 0)) 
             LEFT OUTER JOIN
             dbo.Stock AS S ON PV.Id = S.ProductVariantId AND S.Id IN
             (SELECT TOP (1) Id
             FROM  dbo.Stock
             WHERE  (PV.Id = ProductVariantId) AND (IsActive = 1))
             WHERE  (P.IsActive = 1) AND (C.IsActive = 1);
             GO");

        }
    }
}
