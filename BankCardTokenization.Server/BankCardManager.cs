using BankCardTokenization.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankCardTokenization.Server
{
    /// <summary>
    /// Class for generating new token
    /// </summary>
    public static class BankCardTokenManager
    {
        /// <summary>
        /// Method for generating the new token
        /// </summary>
        /// <param name="bankCardNumber">Bank card number</param>
        /// <returns>Generated token</returns>
        public static string GenerateToken(string bankCardNumber)
        {
            //Check if card number is valid
            if (!IsBankCardNumberValid(bankCardNumber))
            {
                //return null if the card number is not valid
                return null;
            }
            else
            {
                //keep last four digits of bank card number
                string lastDigits = bankCardNumber.Substring(bankCardNumber.Length - 4);

                //return the generated token + the last four digits
                return CreateToken(bankCardNumber) + lastDigits;
            }
        }

        /// <summary>
        /// Method for generating the token
        /// </summary>
        /// <param name="bankCardNumber">The bank card number</param>
        /// <returns></returns>
        private static string CreateToken(string bankCardNumber)
        {
            //create new array for result
            int[] result = new int[bankCardNumber.Length - 4];
            //Random generator of numbers
            Random rand = new Random((int)DateTime.Now.Ticks);

            do
            {
                //generate first digit of token according to rule
                result[0] = GenerateRandomNumber(int.Parse(bankCardNumber[0].ToString()), rand);
            }
            while (!IsStartDigitCorrect(result[0]));

            //Generate all other numbers in the token
            for (int i = 1; i < result.Length; i++)
            {
                result[i] = GenerateRandomNumber(int.Parse(bankCardNumber[i].ToString()), rand);
            }

            //join the numbers of the token to get a string
            string token = string.Join("", result);

            //while the token is not valid change one number from the token (random number is selected)
            while(!IsBankCardNumberValid(token + bankCardNumber.Substring(bankCardNumber.Length - 4)))
            {
                int index = rand.Next(1, token.Length - 1);
                result[index] = GenerateRandomNumber(int.Parse(bankCardNumber[index].ToString()), rand);
                token = string.Join("", result);
            }

            //return the generated token
            return token;
        }

        /// <summary>
        /// Method for generating a random number different from the old one.
        /// </summary>
        /// <param name="number">The old number</param>
        /// <param name="rand">The random generator</param>
        /// <returns></returns>
        private static int GenerateRandomNumber(int number, Random rand)
        {
            int newNumber = 0;

            do
            {
                newNumber = rand.Next(1, 10);
            }
            while (newNumber == number);

            return newNumber;
        }

        /// <summary>
        /// Method for validationg if the bank card number is valid
        /// </summary>
        /// <param name="bankCardNumber"></param>
        /// <returns></returns>
        private static bool IsBankCardNumberValid(string bankCardNumber)
        {
            return bankCardNumber.Length == Constants.BANK_CARD_NUMBER_LENGTH 
                && IsStartDigitCorrect(int.Parse(bankCardNumber[0].ToString()))
                && TestLunh(bankCardNumber);
        }

        /// <summary>
        /// Validate the bank card number by the Luhn algorithm
        /// </summary>
        /// <param name="bankCardNumber"></param>
        /// <returns></returns>
        private static bool TestLunh(string bankCardNumber)
        {
            //get all digits of the bank card number
            int[] digits = bankCardNumber.Select(e => int.Parse(e.ToString())).ToArray();
            int sum = 0;

            for (var i = 0; i < digits.Length; i++)
            {
                if (i % 2 != 0)
                {
                    digits[i] *= 2;
                }

                if (digits[i] >= 10)
                {
                    digits[i] = (digits[i] / 10) + (digits[i] % 10);
                }

                sum += digits[i];
            }

            return sum % 10 == 0;
        }

        /// <summary>
        /// Check if the first digit of the bank card number is 3, 4, 5 or 6
        /// </summary>
        /// <param name="digit"></param>
        /// <returns></returns>
        private static bool IsStartDigitCorrect(int digit)
        {
            return digit == 3 || digit == 4 || digit == 5 || digit == 6;
        }
    }
}