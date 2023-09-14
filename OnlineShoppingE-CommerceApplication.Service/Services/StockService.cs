using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Service.Database;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Provider.Extensions;
using Microsoft.EntityFrameworkCore;

namespace OnlineShoppingE_CommerceApplication.Service.Services
{
    public class StockService : IStockService
    {
        private readonly OnlineShoppingDbContext dbContext;
        public StockService(OnlineShoppingDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> AddStock(List<StockDto> stockDetail)
        {
            var transaction = dbContext.Database.BeginTransaction();
            try
            {
                foreach (var item in stockDetail)
                {
                    Stock stock = new Stock();
                    var stockExists = await dbContext.Stock.FirstOrDefaultAsync(x => x.ProductVariantId == item.ProductVariantId && x.CostPrice == item.CostPrice && x.SellingPrice == item.SellingPrice);
                    if (stockExists == null)
                    {
                        stock.CreatedAt = DateTime.Now;
                        stock.IsActive = true;
                        stock.ProductVariantId = item.ProductVariantId;
                        stock.StockToSale = item.Quantity;
                        stock.AvailableQuantity = item.Quantity;
                        stock.CostPrice = item.CostPrice;
                        stock.SellingPrice = item.SellingPrice;
                        dbContext.Stock.Add(stock);
                    }
                    else
                    {
                        stockExists.IsActive = true;
                        stockExists.UpdatedAt = DateTime.Now;
                        stockExists.StockToSale += item.Quantity;
                        stockExists.AvailableQuantity += item.Quantity;
                        stock.StockToSale += item.Quantity;
                    }
                    await dbContext.SaveChangesAsync();
                }
                transaction.Commit();
                return true;

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }
        public async Task<StockInfoDto> GetDetails(StockQuery query)
        {
            StockInfoDto stock = new StockInfoDto();
            var data = dbContext.Stock.Where(s => s.ProductVariantId > 0 && (query.ProductVariantId > 0 ? s.ProductVariantId == query.ProductVariantId : true));
            if (data.Count() == 0)
                return null;
            if (query.OrderBy != null)
                data = QueryableExtensions.OrderBy(data, query.OrderBy);
            stock.TotalRecords = data.Count();
            stock.StockDetails = new List<StockDetail>();
            if (query.IsPagination)
            {
                int TotalPages = (int)Math.Ceiling(stock.TotalRecords / (double)query.PageSize);
                var items = data.Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToList();
                foreach (var item in items)
                {
                    stock.StockDetails.Add(new StockDetail
                    {
                        Id = item.Id,
                        ProductVariantId = item.ProductVariantId,
                        AvailableQuantity = item.AvailableQuantity,
                        CostPrice = item.CostPrice,
                        CreatedAt = item.CreatedAt,
                        IsActive = item.IsActive,
                        SellingPrice = item.SellingPrice,
                        StockToSale = item.StockToSale,
                        UpdatedAt = item.UpdatedAt
                    });
                }
            }
            else
            {
                foreach (var item in data)
                {
                    stock.StockDetails.Add(new StockDetail
                    {
                        Id = item.Id,
                        ProductVariantId = item.ProductVariantId,
                        AvailableQuantity = item.AvailableQuantity,
                        CostPrice = item.CostPrice,
                        CreatedAt = item.CreatedAt,
                        IsActive = item.IsActive,
                        SellingPrice = item.SellingPrice,
                        StockToSale = item.StockToSale,
                        UpdatedAt = item.UpdatedAt
                    });
                }
            }
            return stock;
        }
    }
}
