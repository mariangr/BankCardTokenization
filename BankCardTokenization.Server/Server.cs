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
    /// <summary>
    /// Used for communication with clients.
    /// </summary>
    internal class Server : IDisposable
    {
        /// <summary>
        /// The thread of the server
        /// </summary>
        private Thread ServerThread { get; set; }

        /// <summary>
        /// Delegate for managing messages
        /// </summary>
        private Action<string> ProcessMessage { get; set; }

        /// <summary>
        /// Delegate for managing errors
        /// </summary>
        private Action<string> ProcessError { get; set; }

        /// <summary>
        /// All client connections
        /// </summary>
        private List<Socket> Connections { get; set; }

        /// <summary>
        /// List of all users
        /// </summary>
        public List<User> Users;

        /// <summary>
        /// List of all cards and tokens
        /// </summary>
        public List<BankCard> BankCards;

        /// <summary>
        /// Constructor for serves
        /// </summary>
        /// <param name="processMessage">Action for managing message</param>
        /// <param name="processError">Actions for managing error messages</param>
        public Server(Action<string> processMessage, Action<string> processError)
        {
            if (processMessage == null || processError == null)
            {
                Environment.Exit(Environment.ExitCode);
            }

            this.ProcessMessage = processMessage;
            this.ProcessError = processError;
            this.Connections = new List<Socket>();
            this.LoadData();
            this.InitializeServer();
        }

        /// <summary>
        /// Method for initializing the server
        /// </summary>
        public void InitializeServer()
        {
            //Start new thread for server
            this.ServerThread = new Thread(new ThreadStart(RunServer));
            this.ServerThread.Start();
        }

        /// <summary>
        /// Method for running the server and accepting connections
        /// </summary>
        public void RunServer()
        {
            TcpListener listener;
            try
            {
                //Initialize the listener and start it
                IPAddress address = IPAddress.Parse(Constants.LOCALHOST);
                listener = new TcpListener(address, Constants.PORT);
                listener.Start();

                //Notify listener has started
                ProcessMessage(Constants.WAITING_FOR_CONNECTIONS);

                while (true)
                {
                    //Get new connection
                    Socket newConnection = listener.AcceptSocket();
                    //save connection
                    this.Connections.Add(newConnection);
                    //create new client processor for the new conection
                    ClientProcessor processor = new ClientProcessor(ProcessMessage, ProcessError, Users, BankCards);

                    //Run client processor on new thread
                    ThreadPool.QueueUserWorkItem(new WaitCallback(processor.ProcessClient), newConnection);

                    //Notify accepted connection
                    ProcessMessage(string.Format(Constants.ACCEPTED_CONNECTION, this.Connections.Where(c => c.Connected).Count()));
                }
            }
            catch (Exception e)
            {
                ProcessError(e.Message);
            }
        }

        /// <summary>
        /// Method to load data of the server (users and cards and tokens)
        /// </summary>
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

        /// <summary>
        /// Generic method to load data from xml files
        /// </summary>
        /// <typeparam name="T">Type of the data stored in the xml file</typeparam>
        /// <param name="type">Type of the data stored in the xml file</param>
        /// <param name="filePath">Path of the file</param>
        /// <param name="data">Reference to object that will store the data in the system</param>
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
                //if file does not exist then create new collection
                data = new T();
            }
            catch (Exception e)
            {
                this.ProcessError(e.Message);
            }
        }

        /// <summary>
        /// Method to save data of the server
        /// </summary>
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

        /// <summary>
        /// Generic method for saving the data in the system
        /// </summary>
        /// <typeparam name="T">Type of the data stored in the xml file</typeparam>
        /// <param name="type">Type of the data stored in the xml file</param>
        /// <param name="filePath">Path of the file</param>
        /// <param name="data">Data to be stored</param>
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

        /// <summary>
        /// Method used to dispose of the server and save all data on close
        /// </summary>
        public void Dispose()
        {
            this.SaveData();
        }

        /// <summary>
        /// Method for exporting Tokens and cards ordered by card
        /// </summary>
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

        /// <summary>
        /// Method for exporting Tokens and cards ordered by token
        /// </summary>
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
