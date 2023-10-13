using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class ProductDetailDto
    {   
        public int Id { get; set; }
        public int WishlistId { get;set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? TotalVariant { get; set; }
        public string? Path { get; set; }
        public double? Price { get; set; }
        
    }
}
