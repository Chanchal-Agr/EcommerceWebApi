using OnlineShoppingE_CommerceApplication.Provider.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class OrderResponseDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string? Address { get; set; }
        public int TotalOrders { get; set; }
        public List<OrderDetailDto>? Orders { get; set; }
    }
    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; }

        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentModes PaymentMode { get; set; }
        public double TotalPrice { get; set; }
        public int TotalItems { get; set; }
        public string? ShippingAddress { get; set; }
        public List<ItemDetail>? ItemDetails { get; set; }
    }
    public class ItemDetail
    {
        public string ProductName { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string? ImagePath { get; set; }
        public string Size { get; set; }
        public string Colour { get; set; }
    }

}
