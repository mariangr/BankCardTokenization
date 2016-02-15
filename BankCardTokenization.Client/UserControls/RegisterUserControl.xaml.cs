using BankCardTokenization.Common;
using System.Windows;
using System.Windows.Controls;

namespace BankCardTokenization.Client.UserControls
{
    public partial class RegisterUserControl : UserControl
    {
        public RegisterDelegate ProcessRegister { get; set; }

        public RegisterUserControl()
        {
            InitializeComponent();

            cbxRights.Items.Add(UserRightsEnum.None);
            cbxRights.Items.Add(UserRightsEnum.Register);
            cbxRights.Items.Add(UserRightsEnum.Request);
            cbxRights.Items.Add(UserRightsEnum.All);
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateUserInput() && ProcessRegister != null)
            {
                ProcessRegister(tbxUsername.Text, tbxPassword.Password, (UserRightsEnum)cbxRights.SelectedItem);
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

            if (string.IsNullOrEmpty(tbxRepeatPassword.Password))
            {
                ShowRequiredWarning(tbxRepeatPassword, "You must enter the password again!");
                return false;
            }

            if (cbxRights.SelectedItem == null)
            {
                ShowRequiredWarning(cbxRights, "User rights are required");
                return false;
            }

            if (tbxPassword.Password != tbxRepeatPassword.Password)
            {
                MessageBox.Show("Passwords don't match!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
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
