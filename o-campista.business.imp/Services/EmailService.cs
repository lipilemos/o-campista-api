using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using o_campista.business.IServices;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace o_campista.business.imp.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken)
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            var fromEmail = _configuration["SendGrid:FromEmail"] ?? "noreply@ocampista.com";
            var frontendUrl = _configuration["App:FrontendUrl"] ?? "https://ocampista.com";

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogWarning("SendGrid:ApiKey não configurado. Email de reset não enviado para {Email}.", toEmail);
                return;
            }

            var resetLink = $"{frontendUrl}/reset-password?token={Uri.EscapeDataString(resetToken)}";

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, "O Campista");
            var to = new EmailAddress(toEmail);
            var subject = "Redefinição de senha — O Campista";
            var htmlBody = $"""
                <!DOCTYPE html>
                <html lang="pt-BR">
                <head>
                  <meta charset="UTF-8" />
                  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                </head>
                <body style="margin:0;padding:0;background:#f5f5f5;font-family:sans-serif;">
                  <table width="100%" cellpadding="0" cellspacing="0" style="background:#f5f5f5;padding:40px 0;">
                    <tr>
                      <td align="center">
                        <table width="480" cellpadding="0" cellspacing="0" style="background:#fff;border-radius:12px;overflow:hidden;box-shadow:0 2px 12px rgba(0,0,0,.08);">
                          <tr>
                            <td style="background:#6e1217;padding:32px;text-align:center;">
                              <h1 style="margin:0;color:#fff;font-size:24px;letter-spacing:1px;">🏕️ O Campista</h1>
                            </td>
                          </tr>
                          <tr>
                            <td style="padding:40px 32px;">
                              <h2 style="margin:0 0 16px;color:#333;font-size:20px;">Redefinição de senha</h2>
                              <p style="margin:0 0 24px;color:#555;line-height:1.6;">
                                Recebemos uma solicitação para redefinir a senha da sua conta.
                                Clique no botão abaixo para criar uma nova senha:
                              </p>
                              <table cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                  <td align="center">
                                    <a href="{resetLink}"
                                       style="display:inline-block;background:#6e1217;color:#fff;text-decoration:none;padding:14px 32px;border-radius:8px;font-size:16px;font-weight:600;">
                                      Redefinir senha
                                    </a>
                                  </td>
                                </tr>
                              </table>
                              <p style="margin:24px 0 0;color:#888;font-size:13px;line-height:1.5;">
                                Se você não solicitou a redefinição de senha, ignore este email.<br/>
                                Este link expira em 1 hora.
                              </p>
                            </td>
                          </tr>
                          <tr>
                            <td style="background:#f9f9f9;padding:20px 32px;border-top:1px solid #eee;text-align:center;">
                              <p style="margin:0;color:#aaa;font-size:12px;">© 2026 O Campista. Todos os direitos reservados.</p>
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                  </table>
                </body>
                </html>
                """;

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlBody);

            _logger.LogInformation("Enviando email de reset de senha para {Email}.", toEmail);

            var response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email de reset enviado com sucesso para {Email}.", toEmail);
            }
            else
            {
                var body = await response.Body.ReadAsStringAsync();
                _logger.LogError("Falha ao enviar email de reset para {Email}. Status: {Status}. Resposta: {Body}",
                    toEmail, response.StatusCode, body);
            }
        }
    }
}
