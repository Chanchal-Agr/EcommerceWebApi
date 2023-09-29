using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShoppingE_CommerceApplication.Provider.Entities
{
    public class ProductVariant : Base
    {
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int ColourId { get; set; }
        public string? Path { get; set; }//multiple
        
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [ForeignKey("SizeId")]
        public Size Size { get; set; }

        [ForeignKey("ColourId")]
        public Colour Colour { get; set; }

        public virtual ICollection<Stock> Stocks { get; set; }
    }
}
