namespace BankCardTokenization.Client.UserControls
{
    /// <summary>
    /// Delegate for generating tokens or requesting bank cards.
    /// </summary>
    /// <param name="bankNumber">The token or the bank card number.</param>
    public delegate void RequestGenerateTokenDelegate(string bankNumber);
}