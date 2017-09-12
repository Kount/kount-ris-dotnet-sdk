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
    /// <b>Version:</b> 7.0.0. <br/>
    /// <b>Copyright:</b> 2011 Kount Inc. All Rights Reserved.<br/>
    /// </summary>
    public class Khash
    {
        /// <summary>
        /// Getting or Setting Config Key used in hashing method
        /// </summary>
        public static string ConfigKey { get; set; }

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
                byte[] computeHash = sha.ComputeHash(enc.GetBytes(plainText + "." + ConfigKey));
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

        /// <summary>
        /// Get Base85 encoded ConfigKey
        /// </summary>
        /// <returns>encoded config key</returns>
        public static string GetBase85ConfigKey(string key)
        {
            if (!String.IsNullOrEmpty(ConfigKey))
            {
                return ConfigKey;
            }

            if (String.IsNullOrEmpty(key))
            {
                return String.Empty;
            }

            string decoded = key;
            try
            {
                decoded = Encoding.UTF8.GetString(Base85.Decode(decoded));
            }
            catch (Exception e)
            {
                throw new Ris.RequestException(e.Message);
            }
            return decoded;
        }
    }
}