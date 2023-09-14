using OnlineShoppingE_CommerceApplication.Service.Database;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using System.Drawing;

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

        public async Task<int> Post(Colour colour)
        {
            colour.IsActive = true;
            var transaction = dbContext.Database.BeginTransaction();
            try
            {
                if (colour.Icon != null)
                {
                    var a = System.IO.Directory.GetCurrentDirectory();
                    var path = Path.Combine(a, "Images\\ColourIcons\\", colour.Icon.FileName);

                    colour.Path = string.Concat("Images\\ColourIcons\\", colour.Icon.FileName);
                    if (colour.Icon.Length > 0)
                    {
                        using (FileStream filestream = System.IO.File.Create(path))
                        {
                            colour.Icon.CopyTo(filestream);
                            filestream.Flush();
                        }
                    }
                    transaction.Commit();
                }
                dbContext.Colour.Add(colour);
                await dbContext.SaveChangesAsync();
                return colour.Id;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return 0;

            }
        }
        public async Task<ColourDto> GetAll(QueryBase query)
        {
            ColourDto colourDto = new ColourDto();
            List<Colour> colours = await dbContext.Colour.Where(e => e.IsActive == true).ToListAsync();
            var count = colours.Count();

            List<ColourDetailDto> colourList = new List<ColourDetailDto>();
            var path = "D:\\Chanchal_Assignment\\OnlineShoppingE-CommerceApplication\\";
            if (query.IsPagination)
            {
                int TotalPages = (int)Math.Ceiling(count / (double)query.PageSize);

                var items = colours.Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToList();


                foreach (var colour in items)
                {
                    ColourDetailDto colourDetails = new ColourDetailDto();
                    colourDetails.Id = colour.Id;
                    colourDetails.Name = colour.Name;
                    if (colour.Path != null)
                        colourDetails.IconPath = string.Concat(path, colour.Path);
                    colourList.Add(colourDetails);
                }
            }
            else
            {
                foreach (var colour in colours)
                {
                    ColourDetailDto colourDetails = new ColourDetailDto();
                    colourDetails.Id = colour.Id;
                    colourDetails.Name = colour.Name;
                    if (colour.Path != null)
                        colourDetails.IconPath = string.Concat(path, colour.Path);
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
                colourToUpdate.Name = colour.Name;
                if (colour.Icon != null)
                {
                    var a = System.IO.Directory.GetCurrentDirectory();
                    if (colourToUpdate.Path != null)
                    {
                        var path = Path.Combine(a, colourToUpdate.Path);
                        System.IO.File.Delete(path);
                    }

                    colourToUpdate.Path = string.Concat("Images\\ColourIcons\\", colour.Icon.FileName);
                    var newPath = Path.Combine(a, "Images\\ColourIcons\\", colour.Icon.FileName);

                    if (colour.Icon.Length > 0)
                    {
                        using (FileStream filestream = System.IO.File.Create(newPath))
                        {
                            colour.Icon.CopyTo(filestream);
                            filestream.Flush();
                        }
                    }
                }
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
        public async Task<ColourDetailDto> GetById(int id )
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
