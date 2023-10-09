using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class ProductInfoDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ColourVariant> ColourVariants { get; set; }
        public bool IsWishlist { get; set; }
        
    }
    public class ColourVariant
    {
        public int ColourId { get; set; }
        public string ColourName { get; set; }
        public List<Variant> Variants { get; set; }
        
    }
    public class Variant
    {
        public int ProductVariantId { get; set; }
        public int SizeId { get; set; }
        public string SizeName { get; set; }
        public string SizeDescription { get; set; }
        public double? Price { get; set; }
        public List<string> Path { get; set; }
        public int? Stock { get; set; }
    }
}
