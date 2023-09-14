using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.CustomException
{
   
    public class InvalidUserException : Exception
    {
        public override string Message
        {
            get
            {
                return "Invalid User!";
            }
        }
    }
}
