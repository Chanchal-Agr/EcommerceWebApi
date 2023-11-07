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
         Task<List<int>> AddProductVariantList(List<ProductVariantDto> variants);
         Task<bool> UpdateStatus(int id, bool status);
         //Task<List<ProductInfoDto>> GetProductVariants();
         Task<ProductVariantResponseDto> GetProductVariants(ProductQuery query);
         Task<bool> UpdateProductVariant(ProductVariantRequestDto variant, int id);
         Task<VariantResponseDto> GetVariantById(int id);
    }
}
