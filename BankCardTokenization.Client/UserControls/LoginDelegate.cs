using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankCardTokenization.Client.UserControls
{
    /// <summary>
    /// Delegate for authenticating the user.
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    public delegate void LoginDelegate(string username, string password);
}
