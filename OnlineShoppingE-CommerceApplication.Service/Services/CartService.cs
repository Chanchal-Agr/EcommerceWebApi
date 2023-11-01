using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Service.Database;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Provider.CustomException;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;

namespace OnlineShoppingE_CommerceApplication.Service.Services
{
    public class CartService : ICartService
    {
        private readonly OnlineShoppingDbContext dbContext;
        public CartService(OnlineShoppingDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<int> Post(CartDto cartDto, int customerId)
        {

            Cart cart = new Cart();
            Cart? cartExists = await dbContext.Cart.FirstOrDefaultAsync(x => x.ProductVariantId == cartDto.ProductVariantId && x.CustomerId == customerId);
            //cart.Price = dbContext.Stock.Where(x => x.ProductVariantId == cartDto.ProductVariantId && x.IsActive == true).Max(x => x.SellingPrice);
            int totalQuantity = dbContext.Stock.Where(x => x.ProductVariantId == cartDto.ProductVariantId && x.IsActive == true).Sum(x => x.StockToSale);
            if (cartExists != null)
            {
                cartExists.Quantity = (cartDto.Quantity + cartExists.Quantity <= totalQuantity) ? cartDto.Quantity + cartExists.Quantity : throw new StockUnavailableException();
                cartExists.UpdatedAt = DateTime.Now;
                await dbContext.SaveChangesAsync();
                return cartExists.Id;
            }
            else
            {
                cart.CustomerId = customerId;
                cart.ProductVariantId = cartDto.ProductVariantId;
                cart.Quantity = (cartDto.Quantity <= totalQuantity) ? cartDto.Quantity : throw new StockUnavailableException();
                //cart.Price = dbContext.Stock.Where(x => x.ProductVariantId == cart.ProductVariantId && x.IsActive == true).Max(x => x.SellingPrice);
                dbContext.Cart.Add(cart);
                await dbContext.SaveChangesAsync();
                return cart.Id;
            }
        }


        public async Task<CartInfoDto> GetDetails(int customerId)
        {
            CartInfoDto cartInfo = new CartInfoDto();
            var cart = await dbContext.Cart.Include(p => p.ProductVariant).ThenInclude(p => p.Product).Where(s => s.CustomerId == customerId).ToListAsync();
            try
            {
                if (cart.IsNullOrEmpty())
                {
                    return null;
                }

                else
                {
                    cartInfo.CartDetails = new List<CartDetail>();
                    foreach (var item in cart)
                    {
                        var data = dbContext.Stock?.Where(x => x.ProductVariantId == item.ProductVariantId && x.IsActive).ToList();
                        cartInfo.CartDetails.Add(new CartDetail
                        {
                            Id = item.Id,
                            ProductVariantId = item.ProductVariantId,
                            Path = item.ProductVariant.Path.Split('|').First(),
                            Price =data.Count>0 ? data.Max(p=>p.SellingPrice):0,
                            Quantity = item.Quantity,
                            ProductId = item.ProductVariant.ProductId,
                            ProductName = item.ProductVariant.Product.Name,
                            Stock = dbContext.Stock?.Where(x => x.ProductVariantId == item.ProductVariantId && x.IsActive).Sum(x => x.StockToSale) ?? 0
                        });
                    }
                    cartInfo.TotalItems = cartInfo.CartDetails.Count();
                    foreach (var item in cartInfo.CartDetails)
                        cartInfo.TotalPrice += item.Price * item.Quantity;

                    return cartInfo;

                }
            }
            catch (Exception e)
            {
                throw new Exception();
            }
        }
        public async Task<bool> Delete(int customerId, int id)
        {
            var cartToDelete = dbContext.Cart.FirstOrDefault(x => x.Id == id && x.CustomerId == customerId);
            if (cartToDelete != null)
            {
                dbContext.Remove(cartToDelete);
                await dbContext.SaveChangesAsync();
                return true;
            }
            else
                return false;
        }
        public async Task<bool> Update(int customerId, int id, int quantity)
        {
            var cartToUpdate = dbContext.Cart.FirstOrDefault(x => x.Id == id && x.CustomerId == customerId);

            if (cartToUpdate != null)
            {
                int TotalQuantity = dbContext.Stock.Where(x => x.ProductVariantId == cartToUpdate.ProductVariantId && x.IsActive == true).Sum(x => x.StockToSale);
                if (quantity <= TotalQuantity)
                {
                    cartToUpdate.UpdatedAt = DateTime.Now;
                    cartToUpdate.Quantity = quantity;
                    await dbContext.SaveChangesAsync();
                    return true;
                }
                else if (quantity == 0)
                {
                    dbContext.Remove(cartToUpdate);
                    await dbContext.SaveChangesAsync();
                    return true;
                }
                else
                    return false;
            }
            else
                return false;

        }

        public async Task<int> GetCartCount(int customerId)
        {
            int cartCount = 0;
            var cart = await dbContext.Cart.Where(s => s.CustomerId == customerId).ToListAsync();
            if (!cart.IsNullOrEmpty())
            {
                cartCount = cart.Count();
            }
            return cartCount;
        }
    }
}

