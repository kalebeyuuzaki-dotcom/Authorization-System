namespace Portifolio.Utils
{
    public static class EmailHelper
    {
        /// <summary>
        /// Mascara e-mail de usuário para confirmação de código.
        /// </summary>
        /// <param name="email">E-mail a ser formatado.</param>
        /// <returns>Retorna o email mascarado (ex: em***@gmail.com).</returns>
        public static string MaskEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                return email;

            var parties = email.Split('@');
            var user = parties[0];
            var domain = parties[1];

            if (user.Length <= 3)
            {
                return $"{user[0]}***@{domain}";
            }

            var start = user.Substring(0, 3);
            var mask = new string('*', user.Length - 3);

            return $"{start}{mask}@{domain}";
        }
    }
}
