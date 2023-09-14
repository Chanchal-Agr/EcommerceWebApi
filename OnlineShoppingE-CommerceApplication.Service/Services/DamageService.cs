using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Service.Database;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OnlineShoppingE_CommerceApplication.Service.Services
{
    public class DamageService : IDamageService
    {
        private readonly OnlineShoppingDbContext dbContext;
        public DamageService(OnlineShoppingDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<int> Post(DamageDto damageStock)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var stock = dbContext.Stock.FirstOrDefault(x => x.Id == damageStock.StockId);
                    if (stock != null && damageStock.Quantity <= stock.StockToSale)
                    {
                        stock.StockToSale -= damageStock.Quantity;
                        stock.AvailableQuantity -= damageStock.Quantity;
                        if (stock.StockToSale == 0)
                            stock.IsActive = false;
                        Damage damage = new Damage();
                        damage.Quantity = damageStock.Quantity;
                        damage.StockId = damageStock.StockId;
                        damage.CreatedAt = DateTime.Now;
                        dbContext.Damage.Add(damage);
                        await dbContext.SaveChangesAsync();
                        transaction.Commit();
                        return damage.Id;
                       
                    }
                    else
                        return 0;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }
        public async Task<bool> Delete(int id)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var damagedStock = await dbContext.Damage.FirstOrDefaultAsync(x => x.Id == id);
                    if (damagedStock != null)
                    {
                        Stock stock = dbContext.Stock.FirstOrDefault(x => x.Id == damagedStock.StockId);
                        stock.StockToSale += damagedStock.Quantity;
                        stock.AvailableQuantity += damagedStock.Quantity;
                        if (stock.IsActive == false)
                            stock.IsActive = true;
                        dbContext.Damage.Remove(damagedStock);
                        await dbContext.SaveChangesAsync();
                        transaction.Commit();
                        return true;
                    }
                    else
                        return false;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }
        public async Task<List<DamageStockDto>> GetAll()
        {
            List<DamageStockDto> damage = new List<DamageStockDto>();
            var damagedStocks = dbContext.Damage.ToList();
            if (damagedStocks.Count==0)
                return null;
            foreach(var item in damagedStocks)
            {
                damage.Add(new DamageStockDto()
                {
                    Id = item.Id,
                    StockId = item.Id,
                    Quantity=item.Quantity,
                });
            }
            return damage;
        }
    }
}
