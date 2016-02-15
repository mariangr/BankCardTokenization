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

        public Token(string number, string username)
        {
            this.Id = number;
            this.User = username;
        }

        public Token()
            : this(string.Empty, string.Empty)
        {

        }

        public Token(Token token)
            : this(token.Id, token.User)
        {

        }
    }
}
