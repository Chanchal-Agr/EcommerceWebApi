using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class UserInfoDto
    {
        public int UserId { get; set; }
        [StringLength(20, MinimumLength = 3)]
        public string? Name { get; set; }

        
        
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }

        
        [RegularExpression(@"^(0|91)?[6-9][0-9]{9}$", ErrorMessage = "Invalid Mobile Number.")]
        public string? Mobile { get; set; }

    }
}
