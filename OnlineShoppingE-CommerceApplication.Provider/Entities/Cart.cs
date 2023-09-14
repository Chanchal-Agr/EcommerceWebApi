using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public User? Customer { get; set; }
        public int ProductVariantId { get; set; }
        [ForeignKey("ProductVariantId")]
        public ProductVariant? ProductVariant { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DefaultValue(null)]
        public DateTime? UpdatedAt { get; set; }

    }
}
