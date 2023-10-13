using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class ProductVariantResponseDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<ProductResponseDto> Products { get; set; }
    }
    public class ProductResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public int WishlistId { get; set; } = 0;
        public List<VariantDto> Variants { get; set; }
    }
    public class VariantDto
    {
        public int Id { get; set; }
        public int SizeId { get; set; }
        public string SizeName { get; set; }
        public int ColourId { get; set; }
        public string ColourName { get; set; }
        public double? Price { get; set; }
        public List<string> Path { get; set; }
        public int? Stock { get; set; }
    }
}
