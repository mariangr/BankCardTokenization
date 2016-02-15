using BankCardTokenization.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankCardTokenization.Client.UserControls
{
    /// <summary>
    /// Delegate for registering new users.
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    /// <param name="rights">User rights</param>
    public delegate void RegisterDelegate(string username, string password, UserRightsEnum rights);
}
