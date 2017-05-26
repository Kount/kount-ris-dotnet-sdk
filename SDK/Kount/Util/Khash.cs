//-----------------------------------------------------------------------
// <copyright file="Khash.cs" company="Keynetics Inc">
//   2011 Kount Inc. All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Util
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Class for creating Kount RIS KHASH encoding payment tokens.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 6.5.0. <br/>
    /// <b>Copyright:</b> 2011 Kount Inc. All Rights Reserved.<br/>
    /// </summary>
    public class Khash
    {
        /// <summary>
        /// Getting or Setting Secret Phrase used in hashing method
        /// </summary>
        public static string Salt { get; set; }

        /// <summary>
        /// Create a Kount hash of a provided payment token. Payment tokens
        /// that can be hashed via this method include: credit card numbers,
        /// Paypal payment IDs, Check numbers, Google Checkout IDs, Bill Me
        /// Later IDs, and Green Dot MoneyPak IDs.
        /// </summary>
        /// <param name="token">String to be hashed</param>
        /// <returns>String Hashed</returns>
        public static string HashPaymentToken(string token)
        {
            string firstSix = token.Length >= 6 ? token.Substring(0, 6) :
                token;
            return firstSix + Hash(token);
        }

        /// <summary>
        /// Hash a gift card payment token using the Kount hashing algorithm.
        /// </summary>
        /// <param name="merchantId">Merchant ID number</param>
        /// <param name="cardNumber">Card number to be hashed</param>
        /// <returns>String Hashed</returns>
        public static string HashGiftCard(int merchantId, string cardNumber)
        {
            return merchantId + Hash(cardNumber);
        }

        /// <summary>
        /// Compute a Kount hash of a given plain text string.
        ///
        /// Preserves the first six characters of the input
        /// so that hasked tokens can be categorized
        /// by Bank Idenfication Number (BIN).
        /// </summary>
        /// <param name="plainText">String to be hashed</param>
        /// <returns>String Hashed</returns>
        public static string Hash(string plainText)
        {
            string a = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int loopMax = 28;
            string mashed = "";

            var enc = new UTF8Encoding();
            using (SHA1 sha = new SHA1CryptoServiceProvider())
            {
                byte[] computeHash = sha.ComputeHash(enc.GetBytes(plainText + "." + Salt));
                string r = BitConverter.ToString(computeHash).Replace("-", "");

                for (int i = 0; i < loopMax; i += 2)
                {
                    int index = int.Parse(
                        r.Substring(i, 7),
                        System.Globalization.NumberStyles.HexNumber) % 36;
                    mashed += a[index];
                }
            }

            return mashed;
        }

        public static string GetBase64Salt()
        {
            string str2 = Salt.Trim();
            try
            {
                str2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(str2));
            }
            catch (Exception e)
            {
                throw new Ris.RequestException(e.Message);
            }
            return str2;
        }

    }
}