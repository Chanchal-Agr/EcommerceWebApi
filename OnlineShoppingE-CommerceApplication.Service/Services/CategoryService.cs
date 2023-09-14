using Microsoft.EntityFrameworkCore;
using OnlineShoppingE_CommerceApplication.Provider.CustomException;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Service.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            List<Category> category = await dbContext.Category.Where(e => e.IsActive == true).ToListAsync();
            var count = category.Count();
            List<CategoryDetailDto> categories = new List<CategoryDetailDto>();
            if (query.IsPagination)
            {
                int TotalPages = (int)Math.Ceiling(count / (double)query.PageSize);

                var items = category.Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToList();


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
                foreach (var item in category)
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
    }
}
