using OnlineShoppingE_CommerceApplication.Provider.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class OrderQuery : QueryBase
    {
        //public int CustomerId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
        public OrderStatus? Status { get; set; }

    }
}
