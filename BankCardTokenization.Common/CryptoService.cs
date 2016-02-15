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
        private static SHA1 sha1 = new SHA1CryptoServiceProvider();

        public static string GetHash(this string value)
        {
            return Convert.ToBase64String(sha1.ComputeHash(Encoding.ASCII.GetBytes(value)));
        }
    }
}
