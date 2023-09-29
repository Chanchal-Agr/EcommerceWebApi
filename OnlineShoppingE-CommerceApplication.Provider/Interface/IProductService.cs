using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.Interface
{
    public interface IProductService
    {
         Task<int> AddProduct(Product product);
         Task<ProductDto> QueryProduct(ProductQuery productQuery,int userId);
         Task<bool> Update(Product product, int id);
         Task<bool> UpdateStatus(int id, bool status);
         Task<ProductInfoDto> GetById(int id);
         Task<int> AddProductAndItsVariant(ProductAndVariantDto dto);
         
    }
}
