using Microsoft.AspNetCore.Hosting;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Service.Database;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace OnlineShoppingE_CommerceApplication.Service.Services;

public class ProductVariantService : IProductVariantService
{
    private readonly OnlineShoppingDbContext dbContext;
    private readonly IWebHostEnvironment environment;
    public ProductVariantService(OnlineShoppingDbContext dbContext, IWebHostEnvironment environment)
    {
        this.dbContext = dbContext;
        this.environment = environment;
    }

    public async Task<bool> Post(List<ProductVariantDto> variants)
    {

        foreach (var item in variants)
        {
            var variantExists = dbContext.ProductVariant.FirstOrDefault(x => x.ColourId == item.ColourId && x.ProductId == item.ProductId && x.SizeId == item.SizeId);
            if (variantExists != null)
            {
                if (variantExists.IsActive)
                    return false;
                else
                {
                    variantExists.IsActive = true;
                    variantExists.Path = String.Join("|", item.Base64);
                    variantExists.UpdatedAt = DateTime.Now;
                    dbContext.SaveChanges();
                }
            }
            else
            {
                ProductVariant productVariant = new ProductVariant();
                productVariant.IsActive = true;
                productVariant.ColourId = item.ColourId;
                productVariant.ProductId = item.ProductId;
                productVariant.SizeId = item.SizeId;
                productVariant.Path = String.Join("|", item.Base64);
                dbContext.ProductVariant.Add(productVariant);
                dbContext.SaveChanges();
            }
        }
        return true;
    }
    public async Task<bool> UpdateStatus(int id, bool status)
    {
        try
        {
            var variantToUpdate = await dbContext.ProductVariant.FirstOrDefaultAsync(e => e.Id == id);
            if (variantToUpdate != null)
            {
                variantToUpdate.UpdatedAt = DateTime.Now;
                variantToUpdate.IsActive = status;
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
    public async Task<List<ProductInfoDto>> GetProductVariants()
    {
        List<ProductInfoDto> list = new List<ProductInfoDto>();
        var products = dbContext.Product.Include(c => c.Category).Where(p => p.IsActive && p.Category.IsActive == true).ToList();
        if (products == null)
            return null;
        foreach (var product in products)
        {
            ProductInfoDto productInfo = new ProductInfoDto();
            productInfo.ColourVariants = new List<ColourVariant>();
            productInfo.Id = product.Id;
            productInfo.CategoryId = product.CategoryId;
            productInfo.CategoryName = product.Category.Name;
            productInfo.Name = product.Name;
            productInfo.Description = product.Description;
            var data = dbContext.ProductVariant.Include(x => x.Size).Include(x => x.Colour).Include(x => x.Stocks).Where(x => x.ProductId == product.Id && x.IsActive == true).GroupBy(x => x.ColourId).ToList();

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
            list.Add(productInfo);
        }
        return list;
    }

    public async Task<ProductVariantResponseDto> GetProductVariants(int categoryId, int customerId)
    {
        ProductVariantResponseDto response = new ProductVariantResponseDto();
        List<ProductResponseDto> productList = new List<ProductResponseDto>();

        var products = dbContext.Product.Include(c => c.Category).Where(p => p.IsActive && p.Category.IsActive && p.CategoryId == categoryId).ToList();

        response.CategoryId = categoryId;
        response.CategoryName = products.FirstOrDefault().Category.Name;
        foreach (var product in products)
        {
            var wishlist = dbContext.Wishlist.FirstOrDefault(x => x.CustomerId == customerId && x.ProductId == product.Id);
            var variantList = dbContext.ProductVariant.Include(x => x.Size).Include(x => x.Colour).Include(x => x.Stocks).Where(x => x.ProductId == product.Id && x.IsActive).ToList();

            List<VariantDto> variants = new List<VariantDto>();
            variants = variantList.Select(p => new VariantDto()
            {
                Id = p.Id,
                Path = p.Path?.Split('|').ToList(),
                SizeId = p.SizeId,
                SizeName = p.Size?.Name,
                ColourId = p.ColourId,
                ColourName = p.Colour?.Name,
                Stock = p.Stocks?.Where(x => x.IsActive).Sum(x => x?.StockToSale) ?? 0,
                Price = p.Stocks?.Where(x => x.IsActive).Max(x => x?.SellingPrice) ?? 0
            }).ToList();

            productList.Add(new ProductResponseDto()
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductDescription = product?.Description,
                WishlistId = wishlist != null ? wishlist.Id : 0,
                Variants = variants
            });

        }
        response.Products = productList;

        return response;

    }
    public async Task<bool> UpdateProductVariant(ProductVariantRequestDto variant, int id)
    {
        try
        {
            var variantToUpdate = await dbContext.ProductVariant.FirstOrDefaultAsync(e => e.Id == id);
            var data = dbContext.ProductVariant.FirstOrDefault(x => x.ProductId == variantToUpdate.ProductId && x.ColourId == variant.ColourId && x.SizeId == variant.SizeId);
            if (data != null && data.Id != id)
                return false;
            else
            {
                variantToUpdate.UpdatedAt = DateTime.Now;
                variantToUpdate.SizeId = variant.SizeId;
                variantToUpdate.ColourId = variant.ColourId;
                variantToUpdate.Path = String.Join("|", variant.Base64);

                await dbContext.SaveChangesAsync();
                return true;
            }
        }
        catch (DbUpdateException exception)
        {
            throw exception;
        }

    }

    public async Task<VariantResponseDto> GetVariantById(int id)
    {
        var variant = await dbContext.ProductVariant.Include(x => x.Size).Include(x => x.Colour).Include(x => x.Product).Include(x=>x.Stocks).FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        if (variant is not null)
        {
            VariantResponseDto variantdto = new VariantResponseDto()
            {
                Id = id,
                ColourId = variant.ColourId,
                SizeId = variant.SizeId,
                ColourName = variant.Colour.Name,
                SizeName = variant.Size.Name,
                ProductName = variant.Product.Name,
                Path = variant.Path?.Split('|').ToList(),
                Price = variant.Stocks?.Where(x => x.IsActive).Max(x => x?.SellingPrice) ?? 0
            };
            return variantdto;
        }
        return null;
    }

}
