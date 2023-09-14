using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class StockDto
    {
        public int Quantity { get; set; }
        public double CostPrice { get; set; }
        public double SellingPrice { get; set; }
        public int ProductVariantId { get; set; }
    }
}
