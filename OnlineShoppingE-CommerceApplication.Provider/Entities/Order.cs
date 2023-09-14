using OnlineShoppingE_CommerceApplication.Provider.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNo { get;set; }
        public DateTime CreatedAt { get; set; }= DateTime.Now;
        [DefaultValue(null)]
        public DateTime? UpdatedAt { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ShippingDate { get; set; } = DateTime.Today.AddDays(7);
        public OrderStatus Status { get; set; } = Provider.Enums.OrderStatus.Placed;
        public PaymentModes PaymentMode { get; set; }
        public double TotalPrice { get; set; }
        public int TotalItems { get; set; }
        public int CustomerId { get;set; }
        [ForeignKey("CustomerId")]
        public User? Customer { get; set; }

        public string? ShippingAddress { get; set; }
    }
}
