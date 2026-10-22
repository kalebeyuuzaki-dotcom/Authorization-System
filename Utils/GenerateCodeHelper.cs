using System.Security.Cryptography;

namespace Portifolio.Utils
{
    public static class GenerateCodeHelper
    {
        /// <summary>
        /// Gera código de confirmação de 6 dígitos.
        /// </summary>
        /// <returns>Retorna o código de 6 dígitos gerado.</returns>
        public static string GenerateConfirmationCode()
        {
            return RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        }
    }
}