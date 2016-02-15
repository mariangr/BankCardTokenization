using BankCardTokenization.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankCardTokenization.Server.Objects
{
    /// <summary>
    /// Class User that represents the User
    /// </summary>
    public class User
    {
        private string username;
        private string password;
        private UserRightsEnum rights;

        /// <summary>
        /// Username of User
        /// </summary>
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

        /// <summary>
        /// Password of User
        /// </summary>
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

        /// <summary>
        /// User rights
        /// </summary>
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

        /// <summary>
        /// General constructor
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="rights"></param>
        public User(string username, string password, UserRightsEnum rights)
        {
            this.Username = username;
            this.Password = password;
            this.Rights = rights;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public User()
            : this(String.Empty, String.Empty, UserRightsEnum.None)
        {

        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="user"></param>
        public User(User user)
            : this(user.Username, user.Password, user.Rights)
        {

        }
    }
}
