using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BankCardTokenization.Common
{
    public static class CryptoService
    {
        /// <summary>
        /// SHA1 Crypto Service Provider
        /// </summary>
        private static SHA1 sha1 = new SHA1CryptoServiceProvider();

        /// <summary>
        /// Extention method for hashing strings
        /// </summary>
        /// <param name="value">string value</param>
        /// <returns></returns>
        public static string GetHash(this string value)
        {
            return Convert.ToBase64String(sha1.ComputeHash(Encoding.ASCII.GetBytes(value)));
        }
    }
}
