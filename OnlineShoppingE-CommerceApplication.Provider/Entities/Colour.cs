using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.Entities
{
    public class Colour : Base
    {
        public string Name { get; set; }
        //[NotMapped]
        //public IFormFile? Icon { get; set; }
        public string? Path { get; set; }
    }
}
