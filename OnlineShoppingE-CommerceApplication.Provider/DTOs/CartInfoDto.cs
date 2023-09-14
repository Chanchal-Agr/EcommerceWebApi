using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class CartInfoDto
    {

        public List<CartDetail> CartDetails { get; set; }
        public int TotalItems { get; set; }
        public double TotalPrice { get; set; }
    }
    public class CartDetail
    {
        public int Id { get; set; } 
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductVariantId { get; set; }
        public double Price { get; set; }
        public string Path { get; set; }
        public int Quantity { get; set; }
         
    }
}
