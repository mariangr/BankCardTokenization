using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankCardTokenization.Common
{
    /// <summary>
    /// User rights
    /// </summary>
    public enum UserRightsEnum
    {
        /// <summary>
        /// User  has no rights in the system.
        /// </summary>
        None = 0,
        /// <summary>
        /// User can generate tokens
        /// </summary>
        GenerateToken = 1,
        /// <summary>
        /// User can request bank card number by token
        /// </summary>
        Request = 2,
        /// <summary>
        /// User has all rights in the system
        /// </summary>
        All = 3
    }
}
