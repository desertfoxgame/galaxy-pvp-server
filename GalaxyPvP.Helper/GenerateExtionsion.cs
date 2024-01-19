using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Extensions
{
    public class GenerateExtionsion
    {
        public static string GenerateRandomCode(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string GeneratePassword(int length, bool includeUpperCase, bool includeSpecialChars, bool includeNumber)
        {
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digitChars = "0123456789";
            const string specialChars = "!@#$%^&*()_-+=<>?";

            string validChars = lowerChars;

            if (includeUpperCase)
                validChars += upperChars;

            if (includeSpecialChars)
                validChars += specialChars;

            if (includeNumber)
                validChars += digitChars;

            char[] password = new char[length];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < length; i++)
                {
                    int index = GetRandomIndex(rng, validChars.Length);
                    password[i] = validChars[index];
                }
            }

            return new string(password);
        }

        static int GetRandomIndex(RNGCryptoServiceProvider rng, int maxValue)
        {
            byte[] randomBytes = new byte[4];
            rng.GetBytes(randomBytes);
            int randomValue = BitConverter.ToInt32(randomBytes, 0);
            return Math.Abs(randomValue % maxValue);
        }
    }
}
