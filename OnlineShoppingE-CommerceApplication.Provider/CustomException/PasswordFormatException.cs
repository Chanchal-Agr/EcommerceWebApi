using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.CustomException
{
    public class PasswordFormatException : Exception
    {
        public override string Message
        {
            get
            {
                return "Minimum length of password should be 8, must contains atleast one alphabet,digit & special character";
            }
        }
    }
}
