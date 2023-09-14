using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class ProductQuery : QueryBase
    {
        public int? CategoryId { get; set; }
        public int? CustomerId { get; set; } = 0;
        
    }
}
