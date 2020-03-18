using System.Text;
using System.Security.Cryptography;

namespace MORR.Core.Session.Crypto
{
    public static class CryptoHelper
    {
        /// <summary>
        /// Generates a hash of the provided string using the SHA256 algorithm.
        /// </summary>
        /// <param name="rawData">The string to be hashed.</param>
        /// <returns>The hashed version of the rawData string.</returns>
        public static string GenerateHash(string rawData)
        {
            using var hash = SHA256.Create();
            var hashedBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            const string convertFormat = "x2";
            var builder = new StringBuilder();

            for (var index = 0; index < hashedBytes.Length; index++)
            {
                builder.Append(hashedBytes[index].ToString(convertFormat));
            }

            return builder.ToString();
        }
    }
}
