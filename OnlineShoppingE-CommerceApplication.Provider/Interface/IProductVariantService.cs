using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.Interface
{
    public interface IProductVariantService
    {
         Task<bool> Post(List<ProductVariantDto> variants);
         Task<bool> UpdateStatus(int id, bool status);
         Task<List<ProductInfoDto>> GetProductVariants();
         Task<ProductVariantResponseDto> GetProductVariants(int categoryId,int customerId);
    }
}
