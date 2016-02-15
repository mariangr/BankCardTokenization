using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankCardTokenization.Common
{
    public enum ActionEnum
    {
        Login = 1,
        Register = 2,
        RegisterToken = 3,
        RequestCardNumber = 4,
        Approved = 5,
        Denied = 6,
        Logout = 7,
        Disconnect = 8
    }
}
