using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShoppingE_CommerceApplication.Provider.Entities
{
    public class Stock: Base
    { 
        public int AvailableQuantity { get; set; }
        public int StockToSale { get; set; }
        public double CostPrice { get; set; }
        public double SellingPrice { get; set; }
        public int ProductVariantId { get; set; }
        [ForeignKey("ProductVariantId")]
        public ProductVariant ProductVariant { get; set; }
    }
}
