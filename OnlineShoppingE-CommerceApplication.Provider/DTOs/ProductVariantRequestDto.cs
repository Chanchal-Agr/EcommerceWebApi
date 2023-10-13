using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class ProductVariantRequestDto
    {
        public int Id { get; set; }
        public int SizeId { get; set; }
        public int ColourId { get; set; }
        public List<string> Base64 { get; set; }
    }
}
