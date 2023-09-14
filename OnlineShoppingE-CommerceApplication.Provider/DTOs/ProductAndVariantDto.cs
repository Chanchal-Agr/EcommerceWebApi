using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class ProductAndVariantDto
    {
        public string ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public int ProductCategory { get; set; }
        //public List<ProductVariantDto>? Variants { get; set; }
        public ProductVariantDto Variants { get; set; }
    }
}
