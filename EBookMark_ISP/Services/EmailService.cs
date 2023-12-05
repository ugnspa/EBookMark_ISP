using Amazon.SimpleEmail.Model;
using Amazon.SimpleEmail;
using Microsoft.Extensions.Options;

namespace EBookMark_ISP.Services
{
    public class EmailService : IEmailService
    {
        private readonly AmazonSimpleEmailServiceClient _client;
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
            _client = new AmazonSimpleEmailServiceClient(_emailSettings.AccessKey, _emailSettings.SecretKey, Amazon.RegionEndpoint.GetBySystemName(_emailSettings.Region));
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var sendRequest = new SendEmailRequest
            {
                Source = _emailSettings.SenderEmail,
                Destination = new Destination
                {
                    ToAddresses = new List<string> { to }
                },
                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = new Body
                    {
                        Text = new Content(body)
                    }
                }
            };

            try
            {
                await _client.SendEmailAsync(sendRequest);
                // Handle the response as needed
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }
    }
}
