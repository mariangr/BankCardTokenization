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
    public class ClientProcessor
    {
        private Socket clientSocket;
        private NetworkStream networkStream;
        private BinaryReader reader;
        private BinaryWriter writer;
        private User user;
        private List<User> Users;
        private List<BankCard> Cards;
        private Action<string> ProcessMessage;
        private Action<string> ProcessError;

        public ClientProcessor(Action<string> processMessage, Action<string> processError, List<User> users, List<BankCard> cards)
        {
            this.ProcessMessage = processMessage;
            this.ProcessError = processError;
            this.Users = users;
            this.Cards = cards;
        }

        public void ProcessClient(object socket)
        {
            try
            {
                clientSocket = (Socket)socket;
                networkStream = new NetworkStream(clientSocket);
                reader = new BinaryReader(networkStream);
                writer = new BinaryWriter(networkStream);

                user = AuthenticateUser();

                while (clientSocket.Connected)
                {
                    ProcessRequest();
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

        private void ProcessRequest()
        {
            ActionEnum currentAction = ((ActionEnum)reader.ReadInt32());

            if (currentAction == ActionEnum.RegisterToken && (user.Rights == UserRightsEnum.Register || user.Rights == UserRightsEnum.All))
            {
                GenerateToken();
            }
            else if (currentAction == ActionEnum.RequestCardNumber && (user.Rights == UserRightsEnum.Request || user.Rights == UserRightsEnum.All))
            {
                RequestCardId();
            }
            else if (currentAction == ActionEnum.Logout)
            {
                this.user = null;
                writer.Write(true);
                user = AuthenticateUser();
            }
            else
            {
                writer.Write((int)ActionEnum.Denied);
            }
        }

        private void GenerateToken()
        {
            writer.Write((int)ActionEnum.Approved);
            string bankCardNumber = reader.ReadString();
            string token = string.Empty;
            token = BankCardTokenManager.GenerateToken(bankCardNumber);

            if (string.IsNullOrEmpty(token))
            {
                writer.Write(Constants.INVALID_BANK_CARD_NUMBER);
                return;
            }

            int maxRetries = 1000;

            while(TokenAlreadyInUse(token) && maxRetries > 0)
            {
                token = BankCardTokenManager.GenerateToken(bankCardNumber);
                maxRetries--;
            }

            if (TokenAlreadyInUse(token))
            {
                writer.Write(Constants.FAILED_TOKEN_GENERATION);
                return;
            }

            AddToken(user.Username, bankCardNumber, token);
            writer.Write(token);
            ProcessMessage(string.Format(Constants.USER_HAS_CREATED_TOKEN, user.Username));
        }

        private void AddToken(string username, string bankCardNumber, string token)
        {
           lock(Cards)
            {
                BankCard current = null;
                current = Cards.FirstOrDefault(c => c.Id == bankCardNumber);
                if (current == null)
                {
                    current = new BankCard(bankCardNumber, new List<Token>());
                    Cards.Add(current);
                }

                current.Tokens.Add(new Token(token, username));
            }
        }

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

        private void RequestCardId()
        {
            writer.Write((int)ActionEnum.Approved);
            string token = reader.ReadString();
            string cardID = Constants.BANK_CARD_NOT_FOUND;

            foreach (BankCard card in Cards)
            {
                if (card.Tokens.Any(tk => tk.Id == token))
                {
                    cardID = card.Id;
                    break;
                }
            }

            writer.Write(cardID);
            ProcessMessage(String.Format(Constants.USER_HAS_REQUESTED_BANK_NUMBER, user.Username, cardID));
        }

        private User AuthenticateUser()
        {
            switch ((ActionEnum)reader.ReadInt32())
            {
                case ActionEnum.Login:
                    return LoginUser();
                case ActionEnum.Register:
                    return RegisterUser();
                default:
                    throw new InvalidOperationException(Constants.INVALID_OPERATION);
            }
        }

        private User LoginUser()
        {
            string username;
            string password;
            User user;
            try
            {
                username = reader.ReadString();
                password = reader.ReadString();

                user = Users.FirstOrDefault(u => u.Username == username && u.Password == password);

                if (user != null)
                {
                    ProcessMessage(string.Format(Constants.USER_LOGGED_IN, username));
                    writer.Write(string.Format(Constants.WELLCOME_IN_THE_SYSTEM, username));
                    return user;
                }
                else
                {
                    writer.Write(Constants.USERNAME_OR_PASSWORD_INCORRECT);
                }
            }
            catch (Exception e)
            {
                writer.Write(e.Message);
            }

            return AuthenticateUser();
        }

        private User RegisterUser()
        {
            string username;
            string password;
            UserRightsEnum rights = UserRightsEnum.None;

            username = reader.ReadString();
            password = reader.ReadString();
            rights = (UserRightsEnum)reader.ReadInt32();

            if (Users.Any(u => u.Username == username))
            {
                writer.Write(string.Format(Constants.USERNAME_ALREADY_IN_USE, username));
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
                writer.Write(message);

                return newUser;
            }
        }
    }
}
