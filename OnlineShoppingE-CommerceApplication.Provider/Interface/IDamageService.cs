using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.Interface
{
    public interface IDamageService
    {
        Task<int> Post(DamageDto damageStock);
        Task<bool> Delete(int id);
        Task<List<DamageStockDto>> GetAll();
    }
}
