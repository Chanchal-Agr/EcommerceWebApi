using OnlineShoppingE_CommerceApplication.Provider.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class UserDto
    //information used in token generation
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Mobile { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }

        
        public Roles Role { get; set; }
    }
}
