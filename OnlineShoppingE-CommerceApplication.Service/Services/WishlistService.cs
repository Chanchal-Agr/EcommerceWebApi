using Microsoft.EntityFrameworkCore;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Service.Database;
using OnlineShoppingE_CommerceApplication.Provider.CustomException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Service.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly OnlineShoppingDbContext dbContext;
        public WishlistService(OnlineShoppingDbContext dbContext)
        {
            this.dbContext = dbContext;

        }
        public async Task<int> Post(int productId,int customerId)
        {
            Wishlist wishlist = new Wishlist();
            wishlist.ProductId = productId;
            wishlist.CustomerId= customerId;
            dbContext.Wishlist.Add(wishlist);
            await dbContext.SaveChangesAsync();
            return wishlist.Id;
        }
        public async Task<WishlistDto> GetWishlist(int customerId)
        {
            
            WishlistDto wishlistDto = new WishlistDto();
                var wishlist = await dbContext.Wishlist.Include(x => x.Customer).Include(x => x.Product).Where(x => x.CustomerId == customerId).ToListAsync();
                if (wishlist == null || wishlist.Count==0)
                    return null;
                wishlistDto.CustomerId = customerId;
                wishlistDto.CustomerName = wishlist.FirstOrDefault().Customer.Name;
                wishlistDto.Wishlist = new List<WishlistItem>();
                foreach (var item in wishlist)
                {
                    var variant = dbContext.ProductVariant.FirstOrDefault(x => x.ProductId == item.ProductId);

                    wishlistDto.Wishlist.Add(new WishlistItem()
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        ProductName = item.Product?.Name,
                        ImagePath = variant?.Path?.Split('|').First() ?? string.Empty
                    });
                }
                return wishlistDto;
        }
        public async Task<bool> Delete(int id,int customerId)
        {
            var productToDelete = dbContext.Wishlist.FirstOrDefault(x => x.Id == id && x.CustomerId == customerId);
            if (productToDelete != null)
            {
                dbContext.Remove(productToDelete);
                await dbContext.SaveChangesAsync();
                return true;
            }
            else
                return false;
        }
    }
}
