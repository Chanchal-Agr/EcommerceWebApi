using Microsoft.AspNetCore.Http;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;


namespace OnlineShoppingE_CommerceApplication.Provider.Interface
{
    public interface IColourService
    {
         Task<int> Post(ColourDto colour);
         Task<ColourResponseDto> GetAll(QueryBase query);
         Task<bool> Update(ColourRequestDto colour, int id);
         Task<bool> UpdateStatus(int id, bool status);
         Task<ColourDetailDto> GetById(int id);
    }
}
