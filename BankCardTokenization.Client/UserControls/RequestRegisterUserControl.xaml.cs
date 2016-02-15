using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace BankCardTokenization.Client.UserControls
{
    /// <summary>
    /// Request register User Control
    /// </summary>
    public partial class RequestRegisterUserControl : UserControl
    {
        /// <summary>
        /// Delegate for managing Generate Token.
        /// </summary>
        public RequestGenerateTokenDelegate ProcessGenerateToken { get; set; }

        /// <summary>
        /// Delegate for managing Request Bank Card Number.
        /// </summary>
        public RequestGenerateTokenDelegate ProcessRequestBankNumber { get; set; }

        /// <summary>
        /// Constructor of RequestRegisterUserControl
        /// </summary>
        public RequestRegisterUserControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Click event for Request Bank Card number by token.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetBankCardNumber_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessRequestBankNumber != null)
            {
                ProcessRequestBankNumber(tbxToken.Text.Replace(" ", "").Replace("_", ""));//Remove unnecessary symbols from mask.
            }
        }

        /// <summary>
        /// Click event for Generate Token
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenerateToken_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessGenerateToken != null)
            {
                ProcessGenerateToken(tbxBankCardNumber.Text.Replace(" ", "").Replace("_", ""));//Remove unnecessary symbols from mask.
            }
        }
    }
}
