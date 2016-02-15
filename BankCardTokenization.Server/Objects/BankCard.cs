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

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        private List<Token> tokens;

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

        public BankCard(string cardNumber, List<Token> tokens)
        {
            this.Id = cardNumber;
            this.Tokens = tokens;
        }

        public BankCard()
            : this(string.Empty, new List<Token>())
        {

        }

        public BankCard(BankCard card)
            : this(card.Id, card.Tokens)
        {

        }
    }
}
