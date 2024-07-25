using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;

namespace RafaelSiteCore.Helpers
{
        public class StringHelpers
        {
                public static string GenerateRandomSalt(int length = 20)
                {
                        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                        StringBuilder res = new StringBuilder();
                        Random rnd = new Random();
                        while (0 < length--)
                        {
                                res.Append(validChars[rnd.Next(validChars.Length)]);
                        }
                        return res.ToString();
                }

                public static string GenerateHash(string input)
                {
                        using (SHA256 sha256Hash = SHA256.Create())
                        {
                                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                                StringBuilder builder = new StringBuilder();
                                for (int i = 0; i < bytes.Length; i++)
                                {
                                        builder.Append(bytes[i].ToString("x2"));
                                }
                                return builder.ToString();
                        }
                }

                public static (string pAppToken, string searchToken) GetAppTokenAndSearchTokenFromString(string input)
                {
                        Regex regex = new Regex(@"^(?<pAppToken>[^:]+):(?<searchToken>.+)$");
                        Match match = regex.Match(input);

                        string pAppToken = string.Empty;
                        string searchToken = string.Empty;
                        if (match.Success)
                        {
                                pAppToken = match.Groups["pAppToken"].Value;
                                searchToken = match.Groups["searchToken"].Value;
                        }

                        return (pAppToken, searchToken);
                }
        }
}
