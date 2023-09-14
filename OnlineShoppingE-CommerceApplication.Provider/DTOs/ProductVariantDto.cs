using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class ProductVariantDto
    {
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int ColourId { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
