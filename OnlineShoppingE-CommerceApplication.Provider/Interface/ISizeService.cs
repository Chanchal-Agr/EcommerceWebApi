using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.Interface
{
    public interface ISizeService
    {

         Task<int> Post(Size size);
         Task<SizeDto> GetAll(QueryBase query);
      
         Task<bool> Update(Size size, int id);
         Task<bool> UpdateStatus(int id, bool status);
         Task<SizeDetailDto> GetById(int id);
    }
}
