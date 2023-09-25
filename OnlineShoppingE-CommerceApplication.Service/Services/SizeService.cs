using Microsoft.EntityFrameworkCore;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Service.Database;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using Microsoft.IdentityModel.Tokens;

namespace OnlineShoppingE_CommerceApplication.Service.Services
{
    public class SizeService : ISizeService
    {
        private readonly OnlineShoppingDbContext dbContext;
        public SizeService(OnlineShoppingDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<int> Post(Provider.Entities.Size size)
        {
            size.Name = size.Name;
            size.Description = size.Description;
            dbContext.Size.Add(size);
            await dbContext.SaveChangesAsync();
            return size.Id;

        }
        public async Task<SizeDto> GetAll(QueryBase query)
        {
            SizeDto sizeDto = new SizeDto();
            var data = dbContext.Size.Where(p => p.IsActive == true && (!query.Search.IsNullOrEmpty() ? (p.Description.Contains(query.Search) || p.Name.Contains(query.Search)) : true)).AsQueryable();
            var count = data.Count();
           
            List<SizeDetailDto> sizeList = new List<SizeDetailDto>();
            if (query.IsPagination)
            {
                int TotalPages = (int)Math.Ceiling(count / (double)query.PageSize);

                var items = data.Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToList();


                foreach (var item in items)
                {
                    SizeDetailDto sizeDetails = new SizeDetailDto();
                    sizeDetails.Id = item.Id;
                    sizeDetails.Name = item.Name;
                    sizeDetails.Description = item.Description;
                    sizeList.Add(sizeDetails);
                }
            }
            else
            {
                foreach (var item in data)
                {
                    SizeDetailDto sizeDetails = new SizeDetailDto();
                    sizeDetails.Id = item.Id;
                    sizeDetails.Name = item.Name;
                    sizeDetails.Description = item.Description;
                    sizeList.Add(sizeDetails);
                }
            }
            sizeDto.SizeDetails= sizeList;
            sizeDto.TotalRecords = count;
            return sizeDto;

        }
        public async Task<bool> Update(Provider.Entities.Size size, int id)
        {
            try
            {
                var sizeToUpdate = await dbContext.Size.FirstAsync(e => e.Id == id);

                sizeToUpdate.UpdatedAt = DateTime.Now;
                sizeToUpdate.Name = size.Name;
                sizeToUpdate.Description = size.Description;
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException exception)
            {
                throw exception;
            }

        }
        public async Task<bool> UpdateStatus(int id, bool status)
        {
            try
            {
                var sizeToUpdate = await dbContext.Size.FirstOrDefaultAsync(e => e.Id == id);
                if (sizeToUpdate != null)
                {
                    sizeToUpdate.UpdatedAt = DateTime.Now;
                    sizeToUpdate.IsActive = status;
                    await dbContext.SaveChangesAsync();
                    return true;
                }
                else
                    return false;
            }
            catch (DbUpdateException exception)
            {
                throw exception;
            }
        }
        public async Task<SizeDetailDto> GetById(int id)
        {
            Size? size = await dbContext.Size.FirstOrDefaultAsync(s=>s.Id == id && s.IsActive==true);
            if (size != null)
                return new SizeDetailDto
                {
                    Id = id,
                    Name = size.Name,
                    Description = size.Description,
                };
            return null;
        }

    }
}
