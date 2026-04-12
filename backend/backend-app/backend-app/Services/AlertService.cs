using backend_app.Context;
using backend_app.DTOs;
using backend_app.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace backend_app.Services
{
    public class AlertService
        (
        AuthenticationUserContext authenticationUserContext,
        ApplicationDBContext applicationDBContext
        )
    {

        public async Task CreateAlert(List<UserDTO> userAddresses, string action)
        {
            var message = "Your tutor allocation has changed.";

            var emailLogs = userAddresses.Select(address => new Notification()
            {
                UserId = address.Id,
                Type = Enums.NotificationType.EMAIL,
                Title = $"{action} Alert From {authenticationUserContext.Username}",
                Message = message
            });

            await applicationDBContext.Notifications.AddRangeAsync(emailLogs);

            await applicationDBContext.SaveChangesAsync(); 

            await SendAlert([.. userAddresses.Select(x => x.Email)], message);
        }


        private async Task SendAlert(List<string> userAddresses, string message)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("Test User", "hambalaluyando9@gmail.com"));
            mimeMessage.To.AddRange(userAddresses.Select(v => new MailboxAddress("Name", v)));
            mimeMessage.Subject = "New Action Alert";

            mimeMessage.Body = new TextPart("plain")
            {
                Text = message,
            };


            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

            await client.AuthenticateAsync("hambalaluyando9@gmail.com", "ukxo tiuz uspv mpwj");

            await client.SendAsync(mimeMessage);
            await client.DisconnectAsync(true);
        }
    }
}
