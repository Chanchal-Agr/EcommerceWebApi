using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class ProductVariantResponseDto
    {
        public List<VariantDto> Variants { get; set; }
        public int TotalRecords { get;set; }
    }
    
    public class VariantDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
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
