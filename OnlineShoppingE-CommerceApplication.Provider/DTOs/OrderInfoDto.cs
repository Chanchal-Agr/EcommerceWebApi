using OnlineShoppingE_CommerceApplication.Provider.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class OrderInfoDto
    {
        public List<OrderInfo>? OrderDetails { get; set; }
        public int TotalRecords { get; set; }
    }
    public class OrderInfo
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; } 
        public OrderStatus Status { get; set; } 
        public PaymentModes PaymentMode { get; set; }
        public double TotalPrice { get; set; }
        public int TotalItems { get; set; }
        public string? ShippingAddress { get; set; }
    }
       
}


