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
    public async Task<int> Post(ProductVariantDto variant)
    {
        ProductVariant productVariant = new ProductVariant();
        productVariant.IsActive = true;
        productVariant.ColourId = variant.ColourId;
        productVariant.ProductId = variant.ProductId;
        productVariant.SizeId = variant.SizeId;
        var transaction = dbContext.Database.BeginTransaction();
        try
        {
            if (variant.Images != null)
            {
                var a = System.IO.Directory.GetCurrentDirectory();
                List<string> paths = new List<string>();
                foreach (var image in variant.Images)
                {
                    var path = Path.Combine(a, "Images\\ProductVariants\\", image.FileName);
                    //productVariant.Path += string.Concat("|Images\\ProductVariants\\", image.FileName);
                    paths.Add(string.Concat("Images\\ProductVariants\\", image.FileName));
                    if (image.FileName.Length > 0)
                    {
                        using (FileStream filestream = System.IO.File.Create(path))
                        {
                            image.CopyTo(filestream);
                            filestream.Flush();
                        }
                    }
                }
                productVariant.Path = String.Join("|", paths);

            }
            dbContext.ProductVariant.Add(productVariant);
            await dbContext.SaveChangesAsync();
            transaction.Commit();
            return productVariant.Id;

        }
        catch (Exception ex)
        {

            transaction.Rollback();
            throw ex;
        }
    }
    public async Task<bool> Post(List<ProductVariantDto> variants)
    {
        var transaction = dbContext.Database.BeginTransaction();
        try
        {
            foreach (var item in variants)
            {
                ProductVariant productVariant = new ProductVariant();
                productVariant.IsActive = true;
                productVariant.ColourId = item.ColourId;
                productVariant.ProductId = item.ProductId;
                productVariant.SizeId = item.SizeId;
                if (item.Images != null)
                {
                    var a = System.IO.Directory.GetCurrentDirectory();
                    List<string> paths = new List<string>();
                    foreach (var image in item.Images)
                    {
                        var path = Path.Combine(a, "Images\\ProductVariants\\", image.FileName);
                        //productVariant.Path += string.Concat("|Images\\ProductVariants\\", image.FileName);
                        paths.Add(string.Concat("Images\\ProductVariants\\", image.FileName));
                        if (image.FileName.Length > 0)
                        {
                            using (FileStream filestream = System.IO.File.Create(path))
                            {
                                image.CopyTo(filestream);
                                filestream.Flush();
                            }
                        }
                    }
                    productVariant.Path = String.Join("|", paths);
                }
                dbContext.ProductVariant.Add(productVariant);
                await dbContext.SaveChangesAsync();
            }
            transaction.Commit();
            return true;
        }
        catch(Exception ex)
        {
            transaction.Rollback();
            return false;
        }
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
