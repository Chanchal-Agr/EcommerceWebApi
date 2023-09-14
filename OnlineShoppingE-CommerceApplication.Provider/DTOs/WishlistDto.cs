using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class WishlistDto
    {
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public List<WishlistItem>? Wishlist { get; set; }
    }
    public class WishlistItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ImagePath { get; set; }
    }
}
