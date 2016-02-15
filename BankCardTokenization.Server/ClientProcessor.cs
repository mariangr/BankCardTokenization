using BankCardTokenization.Common;
using BankCardTokenization.Server.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BankCardTokenization.Server
{
    /// <summary>
    /// Management for each of the client's actions
    /// </summary>
    public class ClientProcessor
    {
        /// <summary>
        /// Client socket. Connection to client.
        /// </summary>
        private Socket clientSocket;

        /// <summary>
        /// Network stream for the client.
        /// </summary>
        private NetworkStream networkStream;

        /// <summary>
        /// Binary reader for client
        /// </summary>
        private BinaryReader BinaryReader;

        /// <summary>
        /// Binary writer for client
        /// </summary>
        private BinaryWriter BunaryWriter;

        /// <summary>
        /// Current authenticated user
        /// </summary>
        private User user;

        /// <summary>
        /// List of all users. (Reference)
        /// </summary>
        private List<User> Users;

        /// <summary>
        /// List of all Bank cards and tokens. (Reference)
        /// </summary>
        private List<BankCard> Cards;

        /// <summary>
        /// Delegate for managing messages
        /// </summary>
        private Action<string> ProcessMessage;

        /// <summary>
        /// Delegate for managing errors
        /// </summary>
        private Action<string> ProcessError;

        /// <summary>
        /// Constructor for ClientProcessor
        /// </summary>
        /// <param name="processMessage">Action for managing messages</param>
        /// <param name="processError">Action for managing errors</param>
        /// <param name="users">Reference to all users list</param>
        /// <param name="cards">Reference to all cards and tokens</param>
        public ClientProcessor(Action<string> processMessage, Action<string> processError, List<User> users, List<BankCard> cards)
        {
            this.ProcessMessage = processMessage;
            this.ProcessError = processError;
            this.Users = users;
            this.Cards = cards;
        }

        /// <summary>
        /// Method for managing user actions
        /// </summary>
        /// <param name="socket"></param>
        public void ProcessClient(object socket)
        {
            try
            {
                clientSocket = (Socket)socket;
                networkStream = new NetworkStream(clientSocket);
                BinaryReader = new BinaryReader(networkStream);
                BunaryWriter = new BinaryWriter(networkStream);

                //Authenticate the user
                user = AuthenticateUser();

                if (user != null)
                {
                    while (clientSocket.Connected)
                    {
                        //While the connection is active process user action
                        if (!ProcessRequest())
                        {
                            //if user disconnects then close the socket connection
                            clientSocket.Close();
                            break;
                        }
                    }
                }
                else
                {
                    //if user disconnects then close the socket connection 
                    clientSocket.Close();
                    ProcessMessage(Constants.CONNECTION_CLOSED);
                }
            }
            catch (Exception e)
            {
                if (ProcessError != null)
                {
                    ProcessError(e.Message);
                }
            }
        }

        /// <summary>
        /// Method for managing user actions
        /// </summary>
        /// <returns>True if user performs another action and False if user disconnects</returns>
        private bool ProcessRequest()
        {
            //Get user action
            ActionEnum currentAction = ((ActionEnum)BinaryReader.ReadInt32());

            if (currentAction == ActionEnum.GenerateToken && (user.Rights == UserRightsEnum.GenerateToken || user.Rights == UserRightsEnum.All))
            {
                //Generate token if user has rights
                GenerateToken();
            }
            else if (currentAction == ActionEnum.RequestCardNumber && (user.Rights == UserRightsEnum.Request || user.Rights == UserRightsEnum.All))
            {
                //request bank card number if user has rights
                RequestCardId();
            }
            else if (currentAction == ActionEnum.Logout)
            {
                //Logout user from the sustem
                ProcessMessage(string.Format(Constants.USER_HAS_LOGGED_OUT, user.Username));
                this.user = null;
                BunaryWriter.Write(true);
                user = AuthenticateUser();
                if(user == null)
                    return false;
            }
            else if (currentAction == ActionEnum.Disconnect)
            {
                //Disconnect client
                ProcessMessage(string.Format(Constants.USER_HAS_DISCONNECTED, user.Username));
                this.user = null;
                return false;
            }
            else
            {
                //User has no rights
                BunaryWriter.Write((int)ActionEnum.Denied);
            }

            //Operation completed successfully
            return true;
        }

        /// <summary>
        /// Generate requested token
        /// </summary>
        private void GenerateToken()
        {
            BunaryWriter.Write((int)ActionEnum.Approved);
            string bankCardNumber = BinaryReader.ReadString();
            string token = string.Empty;
            token = BankCardTokenManager.GenerateToken(bankCardNumber);

            if (string.IsNullOrEmpty(token))
            {
                BunaryWriter.Write(Constants.INVALID_BANK_CARD_NUMBER);
                return;
            }

            int maxRetries = 1000;

            while(TokenAlreadyInUse(token) && maxRetries > 0)
            {
                //Generate new token if this one is used
                token = BankCardTokenManager.GenerateToken(bankCardNumber);
                maxRetries--;
            }

            //Check if the generation was successfull
            if (TokenAlreadyInUse(token))
            {
                //Notify generation was unsuccessful
                BunaryWriter.Write(Constants.FAILED_TOKEN_GENERATION);
                return;
            }

            //Add token to existing collection
            AddToken(user.Username, bankCardNumber, token);
            BunaryWriter.Write(token);
            ProcessMessage(string.Format(Constants.USER_HAS_CREATED_TOKEN, user.Username));
        }

        /// <summary>
        /// Method to add the new token in the list
        /// </summary>
        /// <param name="username">User that created the token</param>
        /// <param name="bankCardNumber">The bank card number</param>
        /// <param name="token">Token to be added</param>
        private void AddToken(string username, string bankCardNumber, string token)
        {
            //Lock since more than one thread use this collection
            lock(Cards)
            {
                BankCard current = null;
                current = Cards.FirstOrDefault(c => c.Id == bankCardNumber);
                if (current == null)
                {
                    //if the card doesn't exist then create it
                    current = new BankCard(bankCardNumber, new List<Token>());
                    Cards.Add(current);
                }

                current.Tokens.Add(new Token(token, username));
            }
        }

        /// <summary>
        /// Method to check if the token is already used
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool TokenAlreadyInUse(string token)
        {
            var result = false;

            Cards.ForEach(c =>
            {
                if (c.Tokens.Any(t => t.Id == token))
                {
                    result = true;
                    return;
                }
            });

            return result;
        }

        /// <summary>
        /// Method for requesting bank card number by token
        /// </summary>
        private void RequestCardId()
        {
            BunaryWriter.Write((int)ActionEnum.Approved);
            string token = BinaryReader.ReadString();
            string cardID = Constants.BANK_CARD_NOT_FOUND;

            foreach (BankCard card in Cards)
            {
                if (card.Tokens.Any(tk => tk.Id == token))
                {
                    cardID = card.Id;
                    break;
                }
            }

            BunaryWriter.Write(cardID);
            ProcessMessage(String.Format(Constants.USER_HAS_REQUESTED_BANK_NUMBER, user.Username, cardID));
        }

        /// <summary>
        /// Method to authenticate the user
        /// </summary>
        /// <returns></returns>
        private User AuthenticateUser()
        {
            switch ((ActionEnum)BinaryReader.ReadInt32())
            {
                case ActionEnum.Login:
                    return LoginUser();
                case ActionEnum.Register:
                    return RegisterUser();
                case ActionEnum.Disconnect:
                    return null;
                default:
                    throw new InvalidOperationException(Constants.INVALID_OPERATION);
            }
        }

        /// <summary>
        /// Method to login the user
        /// </summary>
        /// <returns></returns>
        private User LoginUser()
        {
            string username;
            string password;
            User user;
            try
            {
                username = BinaryReader.ReadString();
                password = BinaryReader.ReadString();

                user = Users.FirstOrDefault(u => u.Username == username && u.Password == password);

                if (user != null)
                {
                    ProcessMessage(string.Format(Constants.USER_LOGGED_IN, username));
                    BunaryWriter.Write(string.Format(Constants.WELLCOME_IN_THE_SYSTEM, username));
                    return user;
                }
                else
                {
                    BunaryWriter.Write(Constants.USERNAME_OR_PASSWORD_INCORRECT);
                }
            }
            catch (Exception e)
            {
                BunaryWriter.Write(e.Message);
            }

            return AuthenticateUser();
        }

        /// <summary>
        /// Method to register new user
        /// </summary>
        /// <returns></returns>
        private User RegisterUser()
        {
            string username;
            string password;
            UserRightsEnum rights = UserRightsEnum.None;

            username = BinaryReader.ReadString();
            password = BinaryReader.ReadString();
            rights = (UserRightsEnum)BinaryReader.ReadInt32();

            if (Users.Any(u => u.Username == username))
            {
                BunaryWriter.Write(string.Format(Constants.USERNAME_ALREADY_IN_USE, username));
                return AuthenticateUser();
            }
            else
            {
                User newUser = new User(username, password, rights);
                lock (Users)
                {
                    Users.Add(newUser);
                }

                string message = string.Format(Constants.USER_SUCCESSFULLY_REGISTERED, username);
                ProcessMessage(message);
                BunaryWriter.Write(message);

                return newUser;
            }
        }
    }
}
