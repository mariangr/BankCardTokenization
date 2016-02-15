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
    public class Client
    {
        private TcpClient TcpClient { get; set; }
        public Action<string> ProcessMessage { get; set; }
        public Action<string> ProcessError { get; set; }
        private NetworkStream NetworkStream { get; set; }
        private BinaryReader BinaryReader { get; set; }
        private BinaryWriter BinaryWriter { get; set; }

        public Client(Action<string> processMessage, Action<string> processError)
        {
            this.ProcessMessage = processMessage;
            this.ProcessError = processError;
            InitializeClient();
        }

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

        internal void RequestBankNumber(string bankNumber, MaskedTextBox textBoxResponse)
        {
            BinaryWriter.Write((int)ActionEnum.RequestCardNumber);
            if ((ActionEnum)BinaryReader.ReadInt32() == ActionEnum.Denied)
            {
                ProcessError(Constants.ACCESS_DENIED);
            }
            else
            {
                BinaryWriter.Write(bankNumber);
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

        internal void GenerateToken(string bankNumber, MaskedTextBox textBoxResponse)
        {
            BinaryWriter.Write((int)ActionEnum.RegisterToken);
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

        public bool Login(string username, string password)
        {
            BinaryWriter.Write((int)ActionEnum.Login);
            BinaryWriter.Write(username);
            BinaryWriter.Write(password.GetHash());

            string response = BinaryReader.ReadString();
            ProcessMessage(response);

            return response.Equals(string.Format(Constants.WELLCOME_IN_THE_SYSTEM, username));
        }

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

        internal bool Logout()
        {
            BinaryWriter.Write((int)ActionEnum.Logout);
            return BinaryReader.ReadBoolean();
        }
    }
}
