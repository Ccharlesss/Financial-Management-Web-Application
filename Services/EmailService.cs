// using MailKit.Net.Smtp;
// using MailKit.Security;
// using MimeKit;
// using Microsoft.Extensions.Options;
// using Microsoft.Extensions.Logging;

// public class EmailService
// {
//     private readonly EmailSettings _emailSettings;
//     private readonly ILogger<EmailService> _logger;

//     // Constructor: Initialize an instance of EmailService.
//     // IOptions<EmailSettings> parameter allows it to retrieve email settings from the configuration.
//     public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
//     {
//         _emailSettings = emailSettings.Value;
//         _logger = logger;

//         // Validate that EmailSettings are properly configured.
//         if (string.IsNullOrEmpty(_emailSettings.SmtpUsername) || string.IsNullOrEmpty(_emailSettings.SmtpPassword))
//         {
//             _logger.LogError("SMTP settings are not configured correctly. Username or password is missing.");
//             throw new ArgumentNullException("SMTP settings are missing. Please check your configuration.");
//         }
//     }

//     // Purpose: Send the email to the address.
//     public void SendEmail(string toEmail, string subject, string body)
//     {
//         if (string.IsNullOrEmpty(toEmail))
//         {
//             _logger.LogError("Recipient email address is null or empty.");
//             throw new ArgumentNullException("Recipient email address cannot be null or empty.");
//         }

//         var message = new MimeMessage();
//         message.From.Add(new MailboxAddress("Support CareApp", _emailSettings.SmtpUsername));
//         message.To.Add(new MailboxAddress("Receiver Name", toEmail));
//         message.Subject = subject;

//         // Create a TextPart object to represent the body of the email and set its content to the provided body.
//         var textPart = new TextPart("plain")
//         {
//             Text = body
//         };
        
//         // Assign the TextPart object to the body property of the MimeMessage.
//         message.Body = textPart;

//         try
//         {
//             using (var client = new SmtpClient())
//             {
//                 // Establish a connection to the SMTP server using the SmtpClient.
//                 _logger.LogInformation($"Connecting to SMTP server {_emailSettings.SmtpServer} on port {_emailSettings.SmtpPort}.");
//                 client.Connect(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);

//                 // Authenticate the web app with the SMTP server to ensure that only authorized users can send emails through it.
//                 _logger.LogInformation("Authenticating with SMTP server.");
//                 client.Authenticate(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);

//                 // Send the email message.
//                 _logger.LogInformation($"Sending email to {toEmail}.");
//                 client.Send(message);

//                 // Disconnect from the SMTP server.
//                 client.Disconnect(true);
//             }
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError($"An error occurred while sending the email: {ex.Message}");
//             throw new InvalidOperationException("Failed to send the email. See inner exception for details.", ex);
//         }
//     }
// }




//////////////////////////
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

public class EmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    // Constructor: Initialize an instance of EmailService.
    // IOptions<EmailSettings> parameter allows it to retrieve email settings from the configuration.
    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;

        // Validate that EmailSettings are properly configured.
        if (string.IsNullOrEmpty(_emailSettings.SmtpUsername) || string.IsNullOrEmpty(_emailSettings.SmtpPassword))
        {
            _logger.LogError("SMTP settings are not configured correctly. Username or password is missing.");
            throw new ArgumentNullException("SMTP settings are missing. Please check your configuration.");
        }
    }

    // Purpose: Send the email to the address.
    public void SendEmail(string toEmail, string subject, string body)
    {
        if (string.IsNullOrEmpty(toEmail))
        {
            _logger.LogError("Recipient email address is null or empty.");
            throw new ArgumentNullException("Recipient email address cannot be null or empty.");
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Support CareApp", _emailSettings.SmtpUsername));
        message.To.Add(new MailboxAddress("Receiver Name", toEmail));
        message.Subject = subject;

        // Create a TextPart object to represent the body of the email and set its content to the provided body.
        var textPart = new TextPart("plain")
        {
            Text = body
        };
        
        // Assign the TextPart object to the body property of the MimeMessage.
        message.Body = textPart;

        try
        {
            using (var client = new SmtpClient())
            {
                // Establish a connection to the SMTP server using the SmtpClient.
                _logger.LogInformation($"Connecting to SMTP server {_emailSettings.SmtpServer} on port {_emailSettings.SmtpPort}.");
                client.Connect(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);

                // Authenticate the web app with the SMTP server to ensure that only authorized users can send emails through it.
                _logger.LogInformation("Authenticating with SMTP server.");
                client.Authenticate(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);

                // Send the email message.
                _logger.LogInformation($"Sending email to {toEmail}.");
                client.Send(message);

                // Disconnect from the SMTP server.
                client.Disconnect(true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while sending the email: {ex.Message}");
            throw new InvalidOperationException("Failed to send the email. See inner exception for details.", ex);
        }
    }
}
