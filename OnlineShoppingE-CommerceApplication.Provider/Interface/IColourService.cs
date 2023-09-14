using Microsoft.AspNetCore.Http;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;


namespace OnlineShoppingE_CommerceApplication.Provider.Interface
{
    public interface IColourService
    {
         Task<int> Post(Colour colour);
         Task<ColourDto> GetAll(QueryBase query);
         Task<bool> Update(Colour colour, int id);
         Task<bool> UpdateStatus(int id, bool status);
         Task<ColourDetailDto> GetById(int id);
    }
}
