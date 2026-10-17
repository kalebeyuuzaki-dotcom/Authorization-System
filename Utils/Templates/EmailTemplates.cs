namespace Portifolio.Utils.Templates
{
    public static class EmailTemplates
    {
        public static string BuildVerificationCodeEmail(string name, string code, int expirationMinutes)
        {
            return $"""
            <div style='background-color: #f8f9fa; padding: 40px 10px; font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif; text-align: center;'>
                <table align='center' border='0' cellpadding='0' cellspacing='0' style='max-width: 500px; width: 100%; background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 6px rgba(0,0,0,0.05); border: 1px solid #e9ecef; overflow: hidden;'>
                    <tr>
                        <td style='padding: 40px 30px; text-align: left;'>
                            
                            <!-- Saudação -->
                            <h3 style='color: #212529; font-size: 20px; font-weight: 700; margin-top: 0; margin-bottom: 16px;'>
                                Olá, {name}!
                            </h3>
                            
                            <!-- Texto de Instrução -->
                            <p style='color: #495057; font-size: 15px; line-height: 1.6; margin-top: 0; margin-bottom: 24px;'>
                                Obrigado por se cadastrar em nosso sistema. Use o código de verificação abaixo para confirmar e ativar a sua conta:
                            </p>
                            
                            <!-- Bloco Centralizado do Código -->
                            <div style='text-align: center; margin-bottom: 24px; padding: 16px; background-color: #f8f9fa; border-radius: 6px; border: 1px dashed #ced4da;'>
                                <h2 style='color: #0d6efd; font-size: 32px; font-weight: 700; letter-spacing: 4px; margin: 0;'>
                                    {code}
                                </h2>
                            </div>
                            
                            <!-- Aviso de Expiração -->
                            <p style='color: #6c757d; font-size: 13px; line-height: 1.5; margin-top: 0; margin-bottom: 0;'>
                                ⏱️ Este código expira em <strong>{expirationMinutes} minutos</strong>. Se você não solicitou esta ação, pode ignorar esta mensagem com segurança.
                            </p>
                            
                        </td>
                    </tr>
                    <!-- Rodapé -->
                    <tr>
                        <td style='background-color: #f1f3f5; padding: 16px 30px; text-align: center;'>
                            <p style='color: #adb5bd; font-size: 11px; margin: 0;'>
                                Esta é uma mensagem automática. Por favor, não responda a este e-mail.
                            </p>
                        </td>
                    </tr>
                </table>
            </div>
            """;
        }

        public static string BuildRecoverCodeEmail(string code, int expirationMinutes)
        {
            return $"""
            <div style='background-color: #f8f9fa; padding: 40px 10px; font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif; text-align: center;'>
                <table align='center' border='0' cellpadding='0' cellspacing='0' style='max-width: 500px; width: 100%; background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 6px rgba(0,0,0,0.05); border: 1px solid #e9ecef; overflow: hidden;'>
                    <tr>
                        <td style='padding: 40px 30px; text-align: left;'>
                            
                            <!-- Texto de Instrução -->
                            <p style='color: #495057; font-size: 15px; line-height: 1.6; margin-top: 0; margin-bottom: 24px;'>
                                Use o código de verificação abaixo para recuperar sua senha:
                            </p>
                            
                            <!-- Bloco Centralizado do Código -->
                            <div style='text-align: center; margin-bottom: 24px; padding: 16px; background-color: #f8f9fa; border-radius: 6px; border: 1px dashed #ced4da;'>
                                <h2 style='color: #0d6efd; font-size: 32px; font-weight: 700; letter-spacing: 4px; margin: 0;'>
                                    {code}
                                </h2>
                            </div>
                            
                            <!-- Aviso de Expiração -->
                            <p style='color: #6c757d; font-size: 13px; line-height: 1.5; margin-top: 0; margin-bottom: 0;'>
                                ⏱️ Este código expira em <strong>{expirationMinutes} minutos</strong>. Se você não solicitou esta ação, pode ignorar esta mensagem com segurança.
                            </p>
                            
                        </td>
                    </tr>
                    <!-- Rodapé -->
                    <tr>
                        <td style='background-color: #f1f3f5; padding: 16px 30px; text-align: center;'>
                            <p style='color: #adb5bd; font-size: 11px; margin: 0;'>
                                Esta é uma mensagem automática. Por favor, não responda a este e-mail.
                            </p>
                        </td>
                    </tr>
                </table>
            </div>
            """;
        }
    }
}
