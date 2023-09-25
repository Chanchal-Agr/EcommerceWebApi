using Microsoft.EntityFrameworkCore;
using OnlineShoppingE_CommerceApplication.Provider.CustomException;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Service.Database;
using OnlineShoppingE_CommerceApplication.Provider.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace OnlineShoppingE_CommerceApplication.Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly OnlineShoppingDbContext dbContext;
        public CategoryService(OnlineShoppingDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<int> Post(Category category)
        {
            category.Name = category.Name;
            dbContext.Category.Add(category);
            await dbContext.SaveChangesAsync();
            return category.Id;

        }
        public async Task<CategoryDto> GetAll(QueryBase query)
        {
            CategoryDto categoryDto = new CategoryDto();
            var data = dbContext.Category.Where(p => p.IsActive==true && (!query.Search.IsNullOrEmpty() ? p.Name.Contains(query.Search) : true)).AsQueryable();

            if (query.OrderBy != null)
                data = QueryableExtensions.OrderBy(data, query.OrderBy);
            
            var count = data.Count();
            List<CategoryDetailDto> categories = new List<CategoryDetailDto>();
            if (query.IsPagination)
            {
                int TotalPages = (int)Math.Ceiling(count / (double)query.PageSize);

                var items = data.Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToList();


                foreach (var item in items)
                {
                    CategoryDetailDto categoryDetails = new CategoryDetailDto();
                    categoryDetails.Id = item.Id;
                    categoryDetails.Name = item.Name;
                    categories.Add(categoryDetails);
                }
            }
            else
            {
                foreach (var item in data)
                {
                    CategoryDetailDto categoryDetails = new CategoryDetailDto();
                    categoryDetails.Id = item.Id;
                    categoryDetails.Name = item.Name;
                    categories.Add(categoryDetails);
                }
            }
            categoryDto.CategoryDetails = categories;
            categoryDto.TotalRecords = count;
            return categoryDto;

        }
        public async Task<bool> Update(Category category, int id)
        {
            try
            {
                var categoryToUpdate = await dbContext.Category.FirstAsync(e => e.Id == id);

                categoryToUpdate.UpdatedAt = DateTime.Now;
                categoryToUpdate.Name = category.Name;
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
                var categoryToUpdate = await dbContext.Category.FirstOrDefaultAsync(e => e.Id == id);
                if (categoryToUpdate != null)
                {
                    categoryToUpdate.UpdatedAt = DateTime.Now;
                    categoryToUpdate.IsActive = status;
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
        public async Task<CategoryDetailDto> GetById(int id)
        {
            Category? category = await dbContext.Category.FirstOrDefaultAsync(c => c.Id == id && c.IsActive==true);
            if (category != null)
                return new CategoryDetailDto
                {
                    Id = id,
                    Name = category.Name,
                };
            return null;
            
        }

        public async Task<int> Upsert(Category category)
        {
            if(category.Id>0)  //update
            {
                  var categoryToUpdate = await dbContext.Category.FirstAsync(e => e.Id == category.Id);

                    categoryToUpdate.UpdatedAt = DateTime.Now;
                    categoryToUpdate.Name = category.Name;
            }
            else  //insert
            {
                category.Name = category.Name;
                dbContext.Category.Add(category);
            }
            await dbContext.SaveChangesAsync();
            return category.Id;
        }
    }
}
