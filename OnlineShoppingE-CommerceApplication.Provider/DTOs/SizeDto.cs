using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class SizeDto
    {
        public List<SizeDetailDto> SizeDetails { get; set; }
        public int TotalRecords { get; set; }
    }
}
