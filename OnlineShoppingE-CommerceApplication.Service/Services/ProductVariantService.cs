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
using Microsoft.IdentityModel.Tokens;
using OnlineShoppingE_CommerceApplication.Provider.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq.Dynamic.Core;

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
                    variantExists.Path = System.String.Join("|", item.Base64);
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
                productVariant.Path = System.String.Join("|", item.Base64);
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

    public async Task<ProductVariantResponseDto> GetProductVariants(ProductQuery query)
    {
        ProductVariantResponseDto response = new ProductVariantResponseDto();
        //var variants = dbContext.ProductVariant.Include(x => x.Size).Include(x => x.Colour).Include(x => x.Stocks).Include(x => x.Product).ThenInclude(x => x.Category).Where(x => x.Product.CategoryId == query.CategoryId && x.Product.Category.IsActive && x.Product.IsActive && x.IsActive).AsQueryable();

        var variants = dbContext.ProductVariant.Include(x => x.Size).Include(x => x.Colour).Include(x => x.Stocks).Include(x => x.Product).ThenInclude(x => x.Category).Where(
            x => x.IsActive && x.Product.IsActive && x.Product.Category.IsActive && (query.CategoryId != null ? x.Product.CategoryId == query.CategoryId : true) && (query.ProductId != null ? x.Product.Id == query.ProductId : true)).AsQueryable();

        if (!variants.Any())
            return null;

        if (query.OrderBy != null)
            variants = QueryableExtensions.OrderBy(variants, query.OrderBy);

        response.TotalRecords = variants.Count();
        response.Variants = new List<VariantDto>();
        if (query.IsPagination)
        {
            int TotalPages = (int)Math.Ceiling(response.TotalRecords / (double)query.PageSize);

            var productVariants = variants.Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToList();

            foreach (var item in productVariants)
            {
                response.Variants.Add(new VariantDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Id = item.Id,
                    SizeId = item.SizeId,
                    SizeName = item.Size.Name,
                    ColourId = item.ColourId,
                    ColourName = item.Colour.Name,
                    Stock = item.Stocks?.Where(x => x.IsActive).Sum(x => x?.StockToSale) ?? 0,
                    Price = item.Stocks?.Where(x => x.IsActive).Max(x => x?.SellingPrice) ?? 0,
                    Path = item.Path.Split("|").ToList()
                    //Path = new List<string>()


                });
            }
        }
        else
        {
            foreach (var item in variants)
            {
                response.Variants.Add(new VariantDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Id = item.Id,
                    SizeId = item.SizeId,
                    SizeName = item.Size.Name,
                    ColourId = item.ColourId,
                    ColourName = item.Colour.Name,
                    Stock = item.Stocks?.Where(x => x.IsActive).Sum(x => x?.StockToSale) ?? 0,
                    Price = item.Stocks?.Where(x => x.IsActive).Max(x => x?.SellingPrice) ?? 0,
                    Path = item.Path.Split("|").ToList()
                    //Path = new List<string>()


                });
            }
        }
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
                variantToUpdate.Path = System.String.Join("|", variant.Base64);

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
        var variant = await dbContext.ProductVariant.Include(x => x.Size).Include(x => x.Colour).Include(x => x.Product).Include(x => x.Stocks).FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        if (variant is not null)
        {
            VariantResponseDto variantdto = new VariantResponseDto()
            {
                Id = id,
                ColourId = variant.ColourId,
                SizeId = variant.SizeId,
                ColourName = variant.Colour.Name,
                SizeName = variant.Size.Name,
                ProductId = variant.ProductId,
                ProductName = variant.Product.Name,
                Path = variant.Path?.Split('|').ToList(),
                Price = variant.Stocks?.Where(x => x.IsActive).Max(x => x?.SellingPrice) ?? 0
            };
            return variantdto;
        }
        return null;
    }


}
