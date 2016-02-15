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
        private Server Server { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.Server = new Server(DisplayMessage,DisplayError);
        }

        private void DisplayMessage(string message)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<string>(DisplayMessage), message);
            }
            else
            {
                this.txtMessages.Text += message.ToString() + '\n';
            }
        }

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

        private void btnExportByToken_Click(object sender, RoutedEventArgs e)
        {
            Server.ExportByToken();
        }

        private void btnExportByBankCard_Click(object sender, RoutedEventArgs e)
        {
            Server.ExportByBankCard();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Server.Dispose();
            base.OnClosing(e);
        }
    }
}
