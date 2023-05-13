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
        //This async method send the encrypted password to the user to have the encrypted password
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            // We use this variable to access to the information in the json
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            //Take the email, password, sever and port of the company to send the email
            string emailPass = config.GetValue<string>("emailServer:Password");
            string fromEmail = config.GetValue<string>("emailServer:EmailAddress");
            string server = config.GetValue<string>("emailServer:Host");
            int port = config.GetValue<int>("emailServer:Port");

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Furry Friend Finder", fromEmail)); //Save the information of the user that sends the email
            emailMessage.To.Add(new MailboxAddress("Receiver", email)); //Save the information of the receiver
            emailMessage.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = message; //save the message that the receiver receives in the email
            emailMessage.Body = builder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(server, port, SecureSocketOptions.StartTls); //Connect to the email service with the server and port
                    await client.AuthenticateAsync(fromEmail, emailPass); // Verifify the information of the author and receiver
                    await client.SendAsync(emailMessage); // send the email
                    await client.DisconnectAsync(true); //Disconnect to the email service
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
