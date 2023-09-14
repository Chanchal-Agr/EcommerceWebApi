using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.Entities
{
    public class VProduct
    {
        [Key]
        public int ProductId { get; set; }

        
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        
        public string Name { get; set; }
        public string Description { get; set; }
        public string? FilePath { get; set; }
        public double? SellingPrice { get; set; }
        public int VariantCount { get; set; }
    }
}
