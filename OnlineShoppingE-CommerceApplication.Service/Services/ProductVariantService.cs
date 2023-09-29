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
            ProductVariant productVariant = new ProductVariant();
            productVariant.IsActive = true;
            productVariant.ColourId = item.ColourId;
            productVariant.ProductId = item.ProductId;
            productVariant.SizeId = item.SizeId;

            productVariant.Path = String.Join("|", item.Base64);
            if (dbContext.ProductVariant.FirstOrDefault(x => x.ColourId == item.ColourId && x.ProductId == item.ProductId && x.SizeId == item.SizeId && x.IsActive==true) != null)
                return false;
            dbContext.ProductVariant.Add(productVariant);
            dbContext.SaveChanges();

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
}
