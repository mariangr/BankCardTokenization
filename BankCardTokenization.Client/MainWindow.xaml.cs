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
    /// <summary>
    /// Main Window
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Client connection to server.
        /// </summary>
        private Client Client { get; set; }

        /// <summary>
        /// Constructor for main window.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            //Initialize the client
            this.Client = new Client(DisplayMessage, DisplayError);

            //Set Login delegate for managing the event of login
            ucLoginControl.ProcessLogin = LoginUser;

            //Set Register delegate for managing the event of register
            ucRegisterControl.ProcessRegister = RegisterUser;

            //Set Generate Toke delegate for managing the event of generating a token
            ucRequestRegisterControl.ProcessGenerateToken = GenerateToken;

            //Set Request Bank Number for managing the event of requesting a bank card number by a given token
            ucRequestRegisterControl.ProcessRequestBankNumber = RequestBankNumber;

            //Mousedown event
            this.MouseDown += MainWindow_MouseDown;
        }

        /// <summary>
        /// Method to request a bank card number by token
        /// </summary>
        /// <param name="token">Token</param>
        private void RequestBankNumber(string token)
        {
            try
            {
                Client.RequestBankNumber(token, ucRequestRegisterControl.tbxBankCardNumber);
            }
            catch (Exception e)
            {
                DisplayError(e.Message);
            }
        }

        /// <summary>
        /// Method to generate a token by a given bank card number.
        /// </summary>
        /// <param name="bankNumber">Bank Card number</param>
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

        /// <summary>
        /// Method for managing errors.
        /// </summary>
        /// <param name="errorDetails">The error message</param>
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
        /// Method for managing messages
        /// </summary>
        /// <param name="message">The message</param>
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

        /// <summary>
        /// Method for managing login for users
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        private void LoginUser(string username, string password)
        {
            if (Client.Login(username, password))
            {
                ToggleControls();
            }
        }

        /// <summary>
        /// Method for managing register of new user.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="rights">User rights</param>
        private void RegisterUser(string username, string password, UserRightsEnum rights)
        {
            if (Client.Register(username, password, rights))
            {
                ToggleControls();
            }
        }

        /// <summary>
        /// Method for managing the visibility of the controls according to the user actions.
        /// </summary>
        private void ToggleControls()
        {
            //Show login/register if user is not authenticated or user has logged out
            this.tcLoginRegister.Visibility = this.tcLoginRegister.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            //Show request/register control if user has authenticated
            this.ucRequestRegisterControl.Visibility = this.ucRequestRegisterControl.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            //Show logout button if user has authenticated
            this.btnLogout.Visibility = this.btnLogout.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        /// <summary>
        /// Custom CanExecute for closing the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingClose_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Custom CanExecute for minimizing the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingMinimize_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Custom command binding for managing Closing the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingClose_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        /// <summary>
        /// Custom command binding for managing Minimizing the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingMinimize_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        /// <summary>
        /// MouseDown event. 
        /// Enables the user to drag the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        /// <summary>
        /// Click event for logout of the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Client.Logout();
            ToggleControls();
        }

        /// <summary>
        /// Cloding event of main window
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            //Dispose of client connection when closing the application
            this.Client.Dispose();
        }
    }
}
