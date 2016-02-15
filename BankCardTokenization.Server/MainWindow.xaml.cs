using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BankCardTokenization.Server
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Server connection instance
        /// </summary>
        private Server Server { get; set; }

        /// <summary>
        /// Constructor for Main Window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.Server = new Server(DisplayMessage,DisplayError);
        }

        /// <summary>
        /// Method for handling messages
        /// </summary>
        /// <param name="message"></param>
        private void DisplayMessage(string message)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<string>(DisplayMessage), message);
            }
            else
            {
                this.txtMessages.Text += string.Format("{0:MM/dd/yy H:mm:ss}: ", DateTime.Now) + message.ToString() + '\n';
            }
        }

        /// <summary>
        /// Method for handling errors
        /// </summary>
        /// <param name="errorDetails"></param>
        private void DisplayError(string errorDetails)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<string>(DisplayError), errorDetails);
            }
            else
            {
                MessageBox.Show(errorDetails, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Click event for exporting tokens and cards ordered by token
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportByToken_Click(object sender, RoutedEventArgs e)
        {
            Server.ExportByToken();
        }

        /// <summary>
        /// Click event for exporting tokens and cards ordered by card
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportByBankCard_Click(object sender, RoutedEventArgs e)
        {
            Server.ExportByBankCard();
        }

        /// <summary>
        /// Closing event of Main Window.
        /// Userd to save all data.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            this.Server.Dispose();
            base.OnClosing(e);
        }
    }
}
