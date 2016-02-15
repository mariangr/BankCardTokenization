using System.Windows;
using System.Windows.Controls;

namespace BankCardTokenization.Client.UserControls
{
    /// <summary>
    /// Login User Control
    /// </summary>
    public partial class LoginUserControl : UserControl
    {
        /// <summary>
        /// Delegate for managing the Login
        /// </summary>
        public LoginDelegate ProcessLogin { get; set; }

        /// <summary>
        /// Constructor of LoginUserControl
        /// </summary>
        public LoginUserControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Click event of login button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateUserInput() && ProcessLogin != null)
            {
                ProcessLogin(tbxUsername.Text, tbxPassword.Password);
            }
        }

        /// <summary>
        /// Validate method for user input
        /// </summary>
        /// <returns>True if input is correct and False if it is not.</returns>
        private bool ValidateUserInput()
        {
            if (string.IsNullOrEmpty(tbxUsername.Text))
            {
                ShowRequiredWarning(tbxUsername, "Username is required");
                return false;
            }

            if (string.IsNullOrEmpty(tbxPassword.Password))
            {
                ShowRequiredWarning(tbxPassword, "Password is required");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Method used to show warnings of required text fields.
        /// </summary>
        /// <param name="control">The text field</param>
        /// <param name="message">Warning message</param>
        private void ShowRequiredWarning(Control control, string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            control.Focus(); //Focus on the required control.
        }
    }
}
