using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class QueryBase 
    {
        public bool IsPagination { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public OrderBy? OrderBy { get; set; } //= new OrderBy() { Order = Order.desc, PropertyName = "Id" };
        public string Search { get; set; } = string.Empty;
    }

    public class OrderBy
    {
        public string PropertyName { get; set; }
        public Order Order { get; set; }
    }

    public enum Order
    {
        asc = 1, desc = 2
    }
    
       
   
}
