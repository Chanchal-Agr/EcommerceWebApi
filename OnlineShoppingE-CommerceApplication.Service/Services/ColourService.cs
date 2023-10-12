using OnlineShoppingE_CommerceApplication.Service.Database;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using System.Drawing;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata.Ecma335;

namespace OnlineShoppingE_CommerceApplication.Service.Services
{
    public class ColourService : IColourService
    {
        private readonly OnlineShoppingDbContext dbContext;
        private readonly IWebHostEnvironment environment;
        public ColourService(OnlineShoppingDbContext dbContext, IWebHostEnvironment environment)
        {
            this.dbContext = dbContext;
            this.environment = environment;
        }

        public async Task<int> Post(ColourDto colour)
        {

            //if (dbContext.Colour.FirstOrDefault(x => x.Name == colour.Name) != null)
            //    return 0;
            //if (colour.Icon != null)
            //{
            //    var a = System.IO.Directory.GetCurrentDirectory();
            //    var path = Path.Combine(a, "Images\\ColourIcons\\", colour.Icon.FileName);
            //    colour.Path = string.Concat("Images\\ColourIcons\\", colour.Icon.FileName);
            //    if (colour.Icon.Length > 0)
            //    {
            //        using (FileStream filestream = System.IO.File.Create(path))
            //        {
            //            colour.Icon.CopyTo(filestream);
            //            filestream.Flush();
            //        }
            //    }
            //}
            Colour color = new Colour();
            if (dbContext.Colour.FirstOrDefault(x => x.Name == colour.Name && x.IsActive) != null)
                return 0;
            else if(dbContext.Colour.FirstOrDefault(x => x.Name == colour.Name && !x.IsActive) != null)
            {
                var item = dbContext.Colour.FirstOrDefault(x => x.Name == colour.Name && !x.IsActive);
                item.UpdatedAt = DateTime.Now;
                item.IsActive= true;
            }
            else { 
            color.Name = colour.Name;
            color.IsActive = true;
            color.Path = colour.Path;
            dbContext.Colour.Add(color);
            
            }
            await dbContext.SaveChangesAsync();
            return color.Id;

        }
        public async Task<ColourResponseDto> GetAll(QueryBase query)
        {
            ColourResponseDto colourDto = new ColourResponseDto();
            var data = dbContext.Colour.Where(p => p.IsActive == true && (!query.Search.IsNullOrEmpty() ? p.Name.Contains(query.Search) : true)).AsQueryable();

            //if (query.OrderBy != null)
            //    data = QueryableExtensions.OrderBy(data, query.OrderBy);
            var count = data.Count();

            List<ColourDetailDto> colourList = new List<ColourDetailDto>();
            var path = "D:\\Chanchal_Assignment\\OnlineShoppingE-CommerceApplication\\";
            if (query.IsPagination)
            {
                int TotalPages = (int)Math.Ceiling(count / (double)query.PageSize);

                var items = data.Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToList();


                foreach (var colour in items)
                {
                    ColourDetailDto colourDetails = new ColourDetailDto();
                    colourDetails.Id = colour.Id;
                    colourDetails.Name = colour.Name;
                    //if (colour.Path != null)
                    //    colourDetails.IconPath = string.Concat(path, colour.Path);
                    colourDetails.IconPath = colour.Path;
                    colourList.Add(colourDetails);
                }
            }
            else
            {
                foreach (var colour in data)
                {
                    ColourDetailDto colourDetails = new ColourDetailDto();
                    colourDetails.Id = colour.Id;
                    colourDetails.Name = colour.Name;
                    //if (colour.Path != null)
                    //    colourDetails.IconPath = string.Concat(path, colour.Path);
                    colourDetails.IconPath = colour.Path;
                    colourList.Add(colourDetails);
                    
                }
            }
            colourDto.ColourDetails = colourList;
            colourDto.TotalRecords = count;
            return colourDto;
        }

        public async Task<bool> Update(Colour colour, int id)
        {
            try
            {

                var colourToUpdate = await dbContext.Colour.FirstAsync(e => e.Id == id && e.IsActive == true);
                colourToUpdate.UpdatedAt = DateTime.Now;
                if (dbContext.Colour.FirstOrDefault(x => x.Name == colour.Name) != null)
                    return false;
                colourToUpdate.Name = colour.Name;
                colourToUpdate.Path = colour.Path;
                //if (colour.Icon != null)
                //{
                //    var a = System.IO.Directory.GetCurrentDirectory();
                //    if (colourToUpdate.Path != null)
                //    {
                //        var path = Path.Combine(a, colourToUpdate.Path);
                //        System.IO.File.Delete(path);
                //    }

                //    colourToUpdate.Path = string.Concat("Images\\ColourIcons\\", colour.Icon.FileName);
                //    var newPath = Path.Combine(a, "Images\\ColourIcons\\", colour.Icon.FileName);

                //    if (colour.Icon.Length > 0)
                //    {
                //        using (FileStream filestream = System.IO.File.Create(newPath))
                //        {
                //            colour.Icon.CopyTo(filestream);
                //            filestream.Flush();
                //        }
                //    }
                //}
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
                var colourToUpdate = await dbContext.Colour.FirstOrDefaultAsync(e => e.Id == id);
                if (colourToUpdate != null)
                {
                    colourToUpdate.UpdatedAt = DateTime.Now;
                    colourToUpdate.IsActive = status;
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
        public async Task<ColourDetailDto> GetById(int id)
        {
            Colour? colour = await dbContext.Colour.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
            if (colour != null)
                return new ColourDetailDto
                {
                    Id = id,
                    Name = colour.Name,
                    IconPath = colour.Path
                };
            return null;
        }
    }
}
