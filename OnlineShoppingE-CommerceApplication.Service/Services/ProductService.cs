using Microsoft.EntityFrameworkCore;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Service.Database;
using System.Linq.Dynamic.Core;
//using OnlineShoppingE_CommerceApplication.Provider.Extensions;
using Microsoft.IdentityModel.Tokens;
using OnlineShoppingE_CommerceApplication.Provider.Extensions;


namespace OnlineShoppingE_CommerceApplication.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly OnlineShoppingDbContext dbContext;
        public ProductService(OnlineShoppingDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<int> AddProduct(Product productDto)
        {

            if (dbContext.Product.FirstOrDefault(x => x.Name == productDto.Name && x.CategoryId == productDto.CategoryId && x.IsActive) != null)
                return 0;
            else if (dbContext.Product.FirstOrDefault(x => x.Name == productDto.Name && x.CategoryId == productDto.CategoryId && !x.IsActive) != null)
            {
                var productExists = dbContext.Product.FirstOrDefault(x => x.Name == productDto.Name && x.CategoryId == productDto.CategoryId && !x.IsActive);
                productExists.UpdatedAt = DateTime.Now;
                productExists.IsActive = true;
                productExists.Description = productDto.Description;
                await dbContext.SaveChangesAsync();
                return productExists.Id;
            }
            else
            {
                Product product = new Product();
                product.Name = productDto.Name;
                product.CategoryId = productDto.CategoryId;
                product.Description = productDto.Description;
                dbContext.Product.Add(product);
                await dbContext.SaveChangesAsync();
                return product.Id;
            }

        }
        // await dbContext.Product.Where(e => e.Id == id).ExecuteUpdateAsync(s => s.SetProperty(status => status.IsActive, false));
        public async Task<bool> Update(Product product, int id)
        {
            try
            {
                if (dbContext.Product.FirstOrDefault(x => x.Name == product.Name && x.CategoryId == product.CategoryId && x.Description == product.Description && x.IsActive && x.Id!=id) != null)
                    return false;
                var productToUpdate = await dbContext.Product.FirstAsync(e => e.Id == id);

                productToUpdate.UpdatedAt = DateTime.Now;
                productToUpdate.Name = product.Name;
                productToUpdate.Description = product.Description;
                productToUpdate.CategoryId = product.CategoryId;

                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException exception)
            {
                throw exception;
            }

        }
        public async Task<bool> UpdateStatus(int id, bool status)
        {
            try
            {
                var productToUpdate = await dbContext.Product.FirstOrDefaultAsync(e => e.Id == id);
                if (productToUpdate != null)
                {
                    productToUpdate.UpdatedAt = DateTime.Now;
                    productToUpdate.IsActive = status;
                    await dbContext.SaveChangesAsync();
                    return true;
                }
                else
                    return false;
            }
            catch (DbUpdateException exception)
            {
                throw exception;
            }
        }
        public async Task<ProductDto> QueryProduct(ProductQuery productQuery, int customerId)
        {
            ProductDto product = new ProductDto();
            var data = dbContext.VProduct.Where(p => p.CategoryId > 0 && (productQuery.CategoryId > 0 ? p.CategoryId == productQuery.CategoryId : true) && (!productQuery.Search.IsNullOrEmpty() ? (p.Description.Contains(productQuery.Search) || p.Name.Contains(productQuery.Search) || p.CategoryName.Contains(productQuery.Search)) : true)).AsQueryable();

            if (productQuery.OrderBy != null)
                data = QueryableExtensions.OrderBy(data, productQuery.OrderBy);
            product.TotalRecords = data.Count();
            product.ProductDetails = new List<ProductDetailDto>();

            if (productQuery.IsPagination)
            {
                int TotalPages = (int)Math.Ceiling(product.TotalRecords / (double)productQuery.PageSize);

                var items = data.Skip((productQuery.PageIndex - 1) * productQuery.PageSize).Take(productQuery.PageSize).ToList();

                foreach (var item in items)
                {
                    var wishlist = dbContext.Wishlist.FirstOrDefault(x => x.CustomerId == customerId && x.ProductId == item.ProductId);
                    product.ProductDetails.Add(new ProductDetailDto
                    {
                        Id = item.ProductId,
                        Category = item.CategoryName,
                        Name = item.Name,
                        Description = item.Description,
                        Price = item.SellingPrice,
                        Path = item.FilePath,
                        TotalVariant = item.VariantCount,
                        WishlistId = wishlist != null ? wishlist.Id : 0
                    });
                }
            }
            else
            {
                var data2 = data.ToList();
                foreach (var item in data2)
                {
                    var wishlist = dbContext.Wishlist.FirstOrDefault(x => x.CustomerId == customerId && x.ProductId == item.ProductId);
                    product.ProductDetails.Add(new ProductDetailDto
                    {
                        Id = item.ProductId,
                        Category = item.CategoryName,
                        Name = item.Name,
                        Description = item.Description,
                        Price = item.SellingPrice,
                        Path = item.FilePath,
                        TotalVariant = item.VariantCount,
                        WishlistId = wishlist != null ? wishlist.Id : 0
                    });
                }
            }
            return product;
        }

        public async Task<ProductInfoDto> GetById(int id, int customerId)
        {
            ProductInfoDto productInfo = new ProductInfoDto();
            var product = dbContext.Product.Include(c => c.Category).FirstOrDefault(p => p.IsActive == true && p.Id == id && p.Category.IsActive == true);
            if (product == null)
                return null;
            else
            {
                var wishlist = dbContext.Wishlist.FirstOrDefault(x => x.CustomerId == customerId && x.ProductId == id);
                productInfo.ColourVariants = new List<ColourVariant>();
                productInfo.Id = product.Id;
                productInfo.CategoryId = product.CategoryId;
                productInfo.CategoryName = product.Category.Name;
                productInfo.Name = product.Name;
                productInfo.Description = product.Description;
                productInfo.WishlistId = wishlist != null ? wishlist.Id : 0;
                var data = dbContext.ProductVariant.Include(x => x.Size).Include(x => x.Colour).Include(x => x.Stocks).Where(x => x.ProductId == id && x.IsActive == true).GroupBy(x => x.ColourId).ToList();

                foreach (var item in data)
                {
                    productInfo.ColourVariants.Add(new ColourVariant()
                    {
                        ColourId = item.Key,
                        ColourName = item.FirstOrDefault()?.Colour.Name,
                        Variants = item.Select(p => new Variant()
                        {
                            Path = p.Path?.Split('|').ToList(),
                            ProductVariantId = p.Id,
                            SizeDescription = p.Size?.Description,
                            SizeId = p.SizeId,
                            SizeName = p.Size?.Name,
                            Stock = p.Stocks?.Where(x => x.IsActive == true).Sum(x => x?.StockToSale) ?? 0,
                            Price = p.Stocks?.Where(x => x.IsActive == true).Max(x => x?.SellingPrice) ?? 0
                        }).ToList()
                    });
                }
                return productInfo;
            }
        }
        //public async Task<int> AddProductAndItsVariant(ProductAndVariantDto dto)
        //{
        //    var transaction = dbContext.Database.BeginTransaction();
        //    try
        //    {
        //        Product product = new Product();
        //        product.CategoryId = dto.ProductCategory;
        //        product.Name = dto.ProductName;
        //        product.Description = dto.ProductDescription;
        //        dbContext.Product.Add(product);
        //        await dbContext.SaveChangesAsync();
        //        ProductVariant productVariant = new ProductVariant();
        //        productVariant.IsActive = true;
        //        productVariant.ColourId = dto.Variants.ColourId;
        //        productVariant.ProductId = product.Id;
        //        productVariant.SizeId = dto.Variants.SizeId;

        //        if (dto.Variants.Images != null)
        //        {
        //            var a = System.IO.Directory.GetCurrentDirectory();
        //            List<string> paths = new List<string>();
        //            foreach (var image in dto.Variants.Images)
        //            {
        //                var path = Path.Combine(a, "Images\\ProductVariants\\", image.FileName);
        //                //productVariant.Path += string.Concat("|Images\\ProductVariants\\", image.FileName);
        //                paths.Add(string.Concat("Images\\ProductVariants\\", image.FileName));
        //                if (image.FileName.Length > 0)
        //                {
        //                    using (FileStream filestream = System.IO.File.Create(path))
        //                    {
        //                        image.CopyTo(filestream);
        //                        filestream.Flush();
        //                    }
        //                }
        //            }
        //            productVariant.Path = String.Join("|", paths);

        //        }
        //        dbContext.ProductVariant.Add(productVariant);
        //        await dbContext.SaveChangesAsync();
        //        transaction.Commit();
        //        return product.Id;
        //    }
        //    catch (Exception ex)
        //    {
        //        transaction.Rollback();
        //        throw ex;
        //    }
        //}
        public async Task<int> AddProductAndItsVariant(ProductAndVariantDto dto)
        {
            var transaction = dbContext.Database.BeginTransaction();
            try
            {
                Product product = new Product();
                product.CategoryId = dto.ProductCategory;
                product.Name = dto.ProductName;
                product.Description = dto.ProductDescription;
                if (dbContext.Product.FirstOrDefault(x => x.CategoryId == dto.ProductCategory && x.Name == dto.ProductName && x.Description == dto.ProductDescription && x.IsActive == true) != null)
                    return 0;
                dbContext.Product.Add(product);
                await dbContext.SaveChangesAsync();
                foreach (var item in dto.Variants)
                {
                    ProductVariant productVariant = new ProductVariant();
                    productVariant.IsActive = true;
                    productVariant.ColourId = item.ColourId;
                    productVariant.ProductId = product.Id;
                    productVariant.SizeId = item.SizeId;

                    productVariant.Path = String.Join("|", item.Base64);
                    if (dbContext.ProductVariant.FirstOrDefault(x => x.ColourId == item.ColourId && x.ProductId == item.ProductId && x.SizeId == item.SizeId && x.IsActive == true) != null)
                        return 0;
                    dbContext.ProductVariant.Add(productVariant);
                    await dbContext.SaveChangesAsync();
                }
                transaction.Commit();
                return product.Id;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }



    }
}
