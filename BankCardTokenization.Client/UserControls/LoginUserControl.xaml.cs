using System.Windows;
using System.Windows.Controls;

namespace BankCardTokenization.Client.UserControls
{

    public partial class LoginUserControl : UserControl
    {
        public LoginDelegate ProcessLogin { get; set; }

        public LoginUserControl()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateUserInput() && ProcessLogin != null)
            {
                ProcessLogin(tbxUsername.Text, tbxPassword.Password);
            }
        }

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

        private void ShowRequiredWarning(Control textBox, string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            textBox.Focus();
        }
    }
}
