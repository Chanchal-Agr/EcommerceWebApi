using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.Entities
{
    public class Category : Base
    {

        [Required]
        public string Name { get; set; }
        public string? Image { get; set; }


    }
}
