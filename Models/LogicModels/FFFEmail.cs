using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;


namespace FurryFriendFinder.Models.LogicModels
{
    public class FFFEmail
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var emailMessage = new MimeMessage();

            string emailPass = config.GetValue<string>("emailServer:Password");
            string fromEmail = config.GetValue<string>("emailServer:EmailAddress");
            string server = config.GetValue<string>("emailServer:Host");
            int port = config.GetValue<int>("emailServer:Port");

            emailMessage.From.Add(new MailboxAddress("Furry Friend Finder", fromEmail));
            emailMessage.To.Add(new MailboxAddress("Nombre del destinatario", email));
            emailMessage.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = message;
            emailMessage.Body = builder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(server, port, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(fromEmail, emailPass);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en el envío de correo electrónico: {ex}");
                    throw;
                }
            }
        }
    }
}
