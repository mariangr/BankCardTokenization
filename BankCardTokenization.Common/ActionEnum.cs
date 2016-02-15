using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankCardTokenization.Common
{
    /// <summary>
    /// User actions in the application
    /// </summary>
    public enum ActionEnum
    {
        /// <summary>
        /// User wants to login into the system
        /// </summary>
        Login = 1,
        /// <summary>
        /// User wants to register in teh system
        /// </summary>
        Register = 2,
        /// <summary>
        /// User wants to Generate a new token in the system
        /// </summary>
        GenerateToken = 3,
        /// <summary>
        /// User wants to request a bank card number by token
        /// </summary>
        RequestCardNumber = 4,
        /// <summary>
        /// System approves the actions of the user
        /// </summary>
        Approved = 5,
        /// <summary>
        /// System denies the actions of the user
        /// </summary>
        Denied = 6,
        /// <summary>
        /// User wants to logout from the system
        /// </summary>
        Logout = 7,
        /// <summary>
        /// Client disconnects from the server
        /// </summary>
        Disconnect = 8
    }
}
