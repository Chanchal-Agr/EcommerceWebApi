using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Provider.Entities;

namespace OnlineShoppingE_CommerceApplication.Provider.Interface;
public interface IStockService
{
     Task<bool> AddStock(List<StockDto> stocks);
     Task<StockInfoDto> GetDetails(StockQuery query);
}
