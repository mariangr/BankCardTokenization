using BankCardTokenization.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankCardTokenization.Server
{
    public static class BankCardTokenManager
    {
        public static string GenerateToken(string bankCardNumber)
        {
            if (!IsBankCardNumberValid(bankCardNumber))
            {
                return null;
            }
            else
            {
                string lastDigits = bankCardNumber.Substring(bankCardNumber.Length - 4);

                return CreateToken(bankCardNumber) + lastDigits;
            }
        }

        private static string CreateToken(string bankCardNumber)
        {
            int[] result = new int[bankCardNumber.Length - 4];
            Random rand = new Random((int)DateTime.Now.Ticks);

            do
            {
                result[0] = GenerateRandomNumber(bankCardNumber[0], rand);
            }
            while (!IsStartDigitCorrect(result[0]));

            for (int i = 1; i < result.Length; i++)
            {
                result[i] = GenerateRandomNumber(bankCardNumber[i], rand);
            }

            string token = string.Join("", result);
            while(!IsBankCardNumberValid(token + bankCardNumber.Substring(bankCardNumber.Length - 4)))
            {
                int index = rand.Next(1, token.Length - 1);
                result[index] = GenerateRandomNumber(bankCardNumber[index], rand);
                token = string.Join("", result);
            }

            return token;
        }

        private static int GenerateRandomNumber(char v, Random rand)
        {
            int newNumber = 0;
            int currentNumber = int.Parse(v.ToString());

            do
            {
                newNumber = rand.Next(1, 10);
            }
            while (newNumber == currentNumber);

            return newNumber;
        }

        private static bool IsBankCardNumberValid(string bankCardNumber)
        {
            return bankCardNumber.Length == Constants.BANK_CARD_NUMBER_LENGTH 
                && IsStartDigitCorrect(int.Parse(bankCardNumber[0].ToString()))
                && TestLunh(bankCardNumber);
        }

        private static bool TestLunh(string bankCardNumber)
        {
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

        private static bool IsStartDigitCorrect(int v)
        {
            return v == 3 || v == 4 || v == 5 || v == 6;
        }


    }
}
