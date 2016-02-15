using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankCardTokenization.Server.Objects
{
    public class BankCard
    {
        private string id;
        private List<Token> tokens;

        /// <summary>
        /// Bank card number
        /// </summary>
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Tokens associated with bank card number
        /// </summary>
        public List<Token> Tokens
        {
            get { return tokens; }
            set
            {
                if (value != null)
                {
                    tokens = new List<Token>(value);
                }
            }
        }

        /// <summary>
        /// General constructor
        /// </summary>
        /// <param name="cardNumber">Bank card number</param>
        /// <param name="tokens">Tokens</param>
        public BankCard(string cardNumber, List<Token> tokens)
        {
            this.Id = cardNumber;
            this.Tokens = new List<Token>(tokens);
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BankCard()
            : this(string.Empty, new List<Token>())
        {

        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="card"></param>
        public BankCard(BankCard card)
            : this(card.Id, card.Tokens)
        {

        }
    }
}
