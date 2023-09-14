using OnlineShoppingE_CommerceApplication.Provider.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class StockInfoDto
    {
        public List<StockDetail>? StockDetails { get; set; }
        public int TotalRecords { get; set; }
    }
    public class StockDetail: Base
    {
        public int AvailableQuantity { get; set; }
        public int StockToSale { get; set; }
        public double CostPrice { get; set; }
        public double SellingPrice { get; set; }
        public int ProductVariantId { get; set; }
    }
}
