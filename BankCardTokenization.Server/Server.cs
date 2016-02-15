using BankCardTokenization.Common;
using BankCardTokenization.Server.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace BankCardTokenization.Server
{
    internal class Server : IDisposable
    {
        private Thread ServerThread { get; set; }
        private Action<string> ProcessMessage { get; set; }
        private Action<string> ProcessError { get; set; }

        public List<User> Users;
        public List<BankCard> BankCards;

        public Server(Action<string> processMessage, Action<string> processError)
        {
            if (processMessage == null || processError == null)
            {
                Environment.Exit(Environment.ExitCode);
            }

            this.ProcessMessage = processMessage;
            this.ProcessError = processError;
            this.LoadData();
            this.InitializeServer();
        }

        public void InitializeServer()
        {
            this.ServerThread = new Thread(new ThreadStart(RunServer));
            this.ServerThread.Start();
        }

        public void RunServer()
        {
            TcpListener listener;
            int connectionCounter = 0;
            try
            {
                IPAddress address = IPAddress.Parse(Constants.LOCALHOST);
                listener = new TcpListener(address, Constants.PORT);
                listener.Start();
                ProcessMessage(Constants.WAITING_FOR_CONNECTIONS);

                while (true)
                {
                    Socket newConnection = listener.AcceptSocket();

                    ClientProcessor processor = new ClientProcessor(ProcessMessage, ProcessError, Users, BankCards);

                    ThreadPool.QueueUserWorkItem(new WaitCallback(processor.ProcessClient), newConnection);

                    ProcessMessage(string.Format(Constants.ACCEPTED_CONNECTION, ++connectionCounter));
                }
            }
            catch (Exception e)
            {
                ProcessError(e.Message);
            }
        }

        private void LoadData()
        {
            try
            {
                LoadXml(typeof(List<BankCard>), Constants.BANK_CARDS_FILE_PATH, ref BankCards);
                LoadXml(typeof(List<User>), Constants.USERS_FILE_PATH, ref Users);

                ProcessMessage(string.Format(Constants.SUCCESSFULLY_READ_DATA, Users.Count, BankCards.Count));
            }
            catch (Exception e)
            {
                this.ProcessError(e.Message);
                this.Users = new List<User>();
                this.BankCards = new List<BankCard>();
            }
        }

        private void LoadXml<T>(Type type, string filePath, ref T data) where T : new()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(type);
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    data = (T)serializer.Deserialize(stream);
                }
            }
            catch (FileNotFoundException)
            {
                data = new T();
            }
            catch (Exception e)
            {
                this.ProcessError(e.Message);
            }
        }

        private void SaveData()
        {
            try
            {
                SaveXml(typeof(List<BankCard>), Constants.BANK_CARDS_FILE_PATH, BankCards);
                SaveXml(typeof(List<User>), Constants.USERS_FILE_PATH, Users);
            }
            catch (Exception e)
            {
                this.ProcessError(e.Message);
            }
        }

        private void SaveXml<T>(Type type, string filePath, List<T> data)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(type);
                using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    serializer.Serialize(stream, data);
                }
            }
            catch (Exception e)
            {

                this.ProcessError(e.Message);
            }
        }

        public void Dispose()
        {
            this.SaveData();
        }

        public void ExportByBankCard()
        {
            string filePath = string.Empty;
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = dialog.FileName;
                }
            }

            try
            {
                List<string> result = new List<string>();
                IEnumerable<BankCard> sorted = BankCards.OrderBy(b => b.Id);
                foreach (BankCard card in sorted)
                {
                    foreach (Token token in card.Tokens)
                    {
                        result.Add(string.Format(Constants.CARDS_TOKENS_EXPORT_TEMPLATE, card.Id, token.Id));
                    }
                }

                File.WriteAllLines(filePath, result);
                ProcessMessage(Constants.EXPORT_SUCCESSFULL);
            }
            catch (Exception e)
            {
                ProcessError(e.Message);
            }
        }

        public void ExportByToken()
        {
            string filePath = string.Empty;
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = dialog.FileName;
                }
            }

            try
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                List<string> result = new List<string>();

                IEnumerable<BankCard> sorted = BankCards.OrderBy(b => b.Id);
                foreach (BankCard card in sorted)
                {
                    foreach (Token token in card.Tokens)
                    {
                        data.Add(token.Id, card.Id);
                    }
                }

                ICollection<string> tokens = data.Keys.OrderBy(k => k).ToList();

                foreach (var token in tokens)
                {
                    result.Add(string.Format(Constants.CARDS_TOKENS_EXPORT_TEMPLATE, data[token], token));
                }

                File.WriteAllLines(filePath, result);
                ProcessMessage(Constants.EXPORT_SUCCESSFULL);
            }
            catch (Exception e)
            {
                ProcessError(e.Message);
            }
        }
    }
}
