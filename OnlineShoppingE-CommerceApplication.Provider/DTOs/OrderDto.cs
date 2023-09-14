using OnlineShoppingE_CommerceApplication.Provider.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class OrderDto
    {
        public PaymentModes PaymentMode { get; set; }
        public string? ShippingAddress { get; set; }
    }
}
