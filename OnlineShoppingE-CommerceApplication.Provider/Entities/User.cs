using OnlineShoppingE_CommerceApplication.Provider.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.Entities
{
    public class User : Base
    {
        
        [Required(ErrorMessage = "First name is required")]
        [StringLength(20, MinimumLength = 3)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Invalid Mobile Number.")]
        [RegularExpression(@"^(0|91)?[6-9][0-9]{9}$", ErrorMessage = "Invalid Mobile Number.")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Field can't be empty")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }

        [Required(ErrorMessage = "Field can't be empty")]
        
        public string Password { get; set; }
        public Roles Role { get; set; }

        public string Otp { get; set; } = String.Empty;

    }
}
