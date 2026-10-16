using System.Security.Cryptography;

namespace Portifolio.Utils
{
    public static class GenerateCodeHelper
    {
        public static string GenerateConfirmationCode()
        {
            return RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        }
    }
}