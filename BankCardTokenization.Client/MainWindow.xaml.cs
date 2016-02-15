using BankCardTokenization.Common;
using System;
using System.Collections.Generic;
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
using System.ComponentModel;

namespace BankCardTokenization.Client
{
    public partial class MainWindow : Window
    {
        private Client Client { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.Client = new Client(DisplayMessage,DisplayError);

            ucLoginControl.ProcessLogin = LoginUser;
            ucRegisterControl.ProcessRegister = RegisterUser;
            ucRequestRegisterControl.ProcessGenerateToken = GenerateToken;
            ucRequestRegisterControl.ProcessRequestBankNumber = RequestBankNumber;

            this.MouseDown += MainWindow_MouseDown;
        }

        private void RequestBankNumber(string bankNumber)
        {
            try
            {
                Client.RequestBankNumber(bankNumber, ucRequestRegisterControl.tbxBankCardNumber);
            }
            catch (Exception e)
            {
                DisplayError(e.Message);
            }
        }

        private void GenerateToken(string bankNumber)
        {
            try
            {
                Client.GenerateToken(bankNumber, ucRequestRegisterControl.tbxToken);
            }
            catch (Exception e)
            {
                DisplayError(e.Message);
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

        private void DisplayMessage(string message)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<string>(DisplayMessage), message);
            }
            else
            {
                MessageBox.Show(message, Constants.INFORMATION_TITLE, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoginUser(string username, string password)
        {
            if (Client.Login(username, password))
            {
                ToggleControls();
            }
        }

        private void RegisterUser(string username, string password, UserRightsEnum rights)
        {
            if (Client.Register(username, password, rights))
            {
                ToggleControls();
            }
        }

        private void ToggleControls()
        {
            this.tcLoginRegister.Visibility = this.tcLoginRegister.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            this.ucRequestRegisterControl.Visibility = this.ucRequestRegisterControl.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            this.btnLogout.Visibility = this.btnLogout.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        private void CommandBindingClose_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBindingMinimize_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBindingClose_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void CommandBindingMinimize_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Client.Logout();
            ToggleControls();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Client.Dispose();
        }
    }
}
