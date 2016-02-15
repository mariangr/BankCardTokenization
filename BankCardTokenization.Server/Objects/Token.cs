using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankCardTokenization.Server.Objects
{
    public class Token
    {
        private string user;
        private string id;

        /// <summary>
        /// User that generated the token
        /// </summary>
        public string User
        {
            get
            {
                return user;
            }
            set
            {
                if (value != null)
                {
                    user = value;
                }
            }
        }

        /// <summary>
        /// Generated token
        /// </summary>
        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                if (value != null)
                {
                    id = value;
                }
            }
        }

        /// <summary>
        /// General constructor
        /// </summary>
        /// <param name="number">Generated token</param>
        /// <param name="username">User that generated token</param>
        public Token(string number, string username)
        {
            this.Id = number;
            this.User = username;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Token()
            : this(string.Empty, string.Empty)
        {

        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="token"></param>
        public Token(Token token)
            : this(token.Id, token.User)
        {

        }
    }
}
