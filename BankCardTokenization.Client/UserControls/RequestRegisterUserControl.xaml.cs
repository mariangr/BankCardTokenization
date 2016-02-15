using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace BankCardTokenization.Client.UserControls
{
    public partial class RequestRegisterUserControl : UserControl
    {
        public RequestGenerateTokenDelegate ProcessGenerateToken { get; set; }
        public RequestGenerateTokenDelegate ProcessRequestBankNumber { get; set; }

        public RequestRegisterUserControl()
        {
            InitializeComponent();
        }

        private void btnGetBankCardNumber_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessRequestBankNumber != null)
            {
                ProcessRequestBankNumber(tbxToken.Text.Replace(" ", "").Replace("_", ""));
            }
        }

        private void btnGenerateToken_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessGenerateToken != null)
            {
                ProcessGenerateToken(tbxBankCardNumber.Text.Replace(" ", "").Replace("_", ""));
            }
        }
    }
}
