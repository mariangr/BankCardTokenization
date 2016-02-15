using BankCardTokenization.Common;
using System.Windows;
using System.Windows.Controls;

namespace BankCardTokenization.Client.UserControls
{
    /// <summary>
    /// Register User Control
    /// </summary>
    public partial class RegisterUserControl : UserControl
    {
        /// <summary>
        /// Delegate for managing the register.
        /// </summary>
        public RegisterDelegate ProcessRegister { get; set; }

        /// <summary>
        /// Constructor of RegisterUserControl
        /// </summary>
        public RegisterUserControl()
        {
            InitializeComponent();

            //Fill all user rights into the combo box
            cbxRights.Items.Add(UserRightsEnum.None);
            cbxRights.Items.Add(UserRightsEnum.GenerateToken);
            cbxRights.Items.Add(UserRightsEnum.Request);
            cbxRights.Items.Add(UserRightsEnum.All);
        }

        /// <summary>
        /// Click event of register button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateUserInput() && ProcessRegister != null)
            {
                ProcessRegister(tbxUsername.Text, tbxPassword.Password, (UserRightsEnum)cbxRights.SelectedItem);
            }
        }

        /// <summary>
        /// Validation of user input.
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

        /// <summary>
        /// Method used to show warnings of required text fields.
        /// </summary>
        /// <param name="control">The text field</param>
        /// <param name="message">Warning message</param>
        private void ShowRequiredWarning(Control control, string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            control.Focus();
        }
    }
}
