using OnlineShoppingE_CommerceApplication.Provider.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class OrderDetailDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int OrderId { get; set; }
        public List<ItemDetail>? ItemDetails { get; set; }
    }
    public class ItemDetail
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string? ImagePath { get; set; }
    }

}
