using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.Interface
{
    public interface ICategoryService
    {
         Task<int> Post(Category category);
         Task<CategoryDto> GetAll(QueryBase query);
         Task<bool> Update(Category category, int id);
         Task<bool> UpdateStatus(int id, bool status);
         Task<CategoryDetailDto> GetById (int id);
         Task<int> Upsert(Category category);


    }
}
