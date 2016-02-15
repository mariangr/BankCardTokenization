using BankCardTokenization.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankCardTokenization.Server.Objects
{
    public class User
    {
        private string username;
        private string password;
        private UserRightsEnum rights;

        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                if (value != null)
                {
                    username = value;
                }
            }
        }

        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                if (value != null)
                {
                    password = value;
                }
            }
        }

        public UserRightsEnum Rights
        {
            get
            {
                return rights;
            }
            set
            {
                rights = value;
            }
        }

        public User(string username, string password, UserRightsEnum rights)
        {
            this.Username = username;
            this.Password = password;
            this.Rights = rights;
        }

        public User()
            : this(String.Empty, String.Empty, UserRightsEnum.None)
        {

        }

        public User(User user)
            : this(user.Username, user.Password, user.Rights)
        {

        }
    }
}
