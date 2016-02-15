using BankCardTokenization.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;

namespace BankCardTokenization.Client
{
    /// <summary>
    /// Used for communication with the server.
    /// </summary>
    public class Client : IDisposable
    {
        /// <summary>
        /// Property for connection to server
        /// </summary>
        private TcpClient TcpClient { get; set; }
        /// <summary>
        /// Delegate for displaying messages.
        /// </summary>
        public Action<string> ProcessMessage { get; set; }
        /// <summary>
        /// Delegate for displaying errors.
        /// </summary>
        public Action<string> ProcessError { get; set; }
        /// <summary>
        /// The network stream of the connection.
        /// </summary>
        private NetworkStream NetworkStream { get; set; }
        /// <summary>
        /// The binary reader of the connection.
        /// </summary>
        private BinaryReader BinaryReader { get; set; }
        /// <summary>
        /// The binary writer of the connection.
        /// </summary>
        private BinaryWriter BinaryWriter { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="processMessage">Action for displaying messages</param>
        /// <param name="processError">Action for displaying errors</param>
        public Client(Action<string> processMessage, Action<string> processError)
        {
            this.ProcessMessage = processMessage;
            this.ProcessError = processError;
            InitializeClient();
        }

        /// <summary>
        /// Method for initializint the Client connection.
        /// </summary>
        private void InitializeClient()
        {
            try
            {
                this.TcpClient = new TcpClient();
                this.TcpClient.Connect(Constants.LOCALHOST, Constants.PORT);

                this.NetworkStream = this.TcpClient.GetStream();
                this.BinaryReader = new BinaryReader(this.NetworkStream);
                this.BinaryWriter = new BinaryWriter(this.NetworkStream);
            }
            catch (Exception e)
            {
                ProcessError(e.Message);
                Environment.Exit(Environment.ExitCode);
            }
        }

        /// <summary>
        /// Method for requesting the bank card number from the server.
        /// </summary>
        /// <param name="token">The token of the request</param>
        /// <param name="textBoxResponse">The textBox that will be filled with the bank card number</param>
        internal void RequestBankNumber(string token, MaskedTextBox textBoxResponse)
        {
            BinaryWriter.Write((int)ActionEnum.RequestCardNumber);
            if ((ActionEnum)BinaryReader.ReadInt32() == ActionEnum.Denied)
            {
                ProcessError(Constants.ACCESS_DENIED);
            }
            else
            {
                BinaryWriter.Write(token);
                string response = BinaryReader.ReadString();
                if (response == Constants.BANK_CARD_NOT_FOUND)
                {
                    ProcessMessage(Constants.BANK_CARD_NOT_FOUND);
                }
                else
                {
                    textBoxResponse.Text = response;
                }
            }
        }

        /// <summary>
        /// Method for generating Tokens by Bank Card number
        /// </summary>
        /// <param name="bankNumber">The number of the Bank Card</param>
        /// <param name="textBoxResponse">The text box that will be filled with the generated token</param>
        internal void GenerateToken(string bankNumber, MaskedTextBox textBoxResponse)
        {
            BinaryWriter.Write((int)ActionEnum.GenerateToken);
            if ((ActionEnum)BinaryReader.ReadInt32() == ActionEnum.Denied)
            {
                ProcessError(Constants.ACCESS_DENIED);
            }
            else
            {
                BinaryWriter.Write(bankNumber);
                string response = BinaryReader.ReadString();

                if (response == Constants.INVALID_BANK_CARD_NUMBER)
                {
                    ProcessError(Constants.INVALID_BANK_CARD_NUMBER);
                }
                else if (response == Constants.FAILED_TOKEN_GENERATION)
                {
                    ProcessError(Constants.FAILED_TOKEN_GENERATION);
                }
                else
                {
                    textBoxResponse.Text = response;
                }
            }
        }

        /// <summary>
        /// Method for login of the user
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        public bool Login(string username, string password)
        {
            BinaryWriter.Write((int)ActionEnum.Login);
            BinaryWriter.Write(username);
            BinaryWriter.Write(password.GetHash());

            string response = BinaryReader.ReadString();
            ProcessMessage(response);

            return response.Equals(string.Format(Constants.WELLCOME_IN_THE_SYSTEM, username));
        }

        /// <summary>
        /// Method for registering new users.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="rights">User Rights</param>
        /// <returns></returns>
        internal bool Register(string username, string password, UserRightsEnum rights)
        {
            BinaryWriter.Write((int)ActionEnum.Register);
            BinaryWriter.Write(username);
            BinaryWriter.Write(password.GetHash());
            BinaryWriter.Write((int)rights);

            string response = BinaryReader.ReadString();
            ProcessMessage(response);

            return response.Equals(string.Format(Constants.USER_SUCCESSFULLY_REGISTERED, username));
        }

        /// <summary>
        /// Method for logout of user.
        /// </summary>
        /// <returns></returns>
        internal bool Logout()
        {
            BinaryWriter.Write((int)ActionEnum.Logout);
            return BinaryReader.ReadBoolean();
        }

        /// <summary>
        /// Method used to disconnect the socket from the server.
        /// </summary>
        public void Dispose()
        {
            this.BinaryWriter.Write((int)ActionEnum.Disconnect);
            this.TcpClient.Close();
        }
    }
}
