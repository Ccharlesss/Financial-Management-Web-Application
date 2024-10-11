using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;

public class EmailService
{
  private readonly EmailSettings _emailSettings;

  // Constructor: Initialoze an instance of EmailService:
  // IOptions<EmailSettings> parameter allows it to retrieve email settings from the configuration:
  public EmailService(IOptions<EmailSettings> emailSettings)
  {
    _emailSettings = emailSettings.Value;
  }

  // Purpose: Send the email to the address:
  public void SendEmail(string toEmail, string subject, string body)
  {
    var message = new MimeMessage();
    message.From.Add(new MailboxAddress("Support CareApp", _emailSettings.SmtpUsername));
    message.To.Add(new MailboxAddress("Receiver Name", toEmail));
    message.Subject = subject;

    // Create a TextPart obj to represent the body of the email and sets its content to the provided body:
    var textPart = new TextPart("plain")
    {
      Text = body
    };
    
    // Assign the TextPart obj to the body property of the MimeMessage:
    message.Body = textPart;

    using (var client = new SmtpClient())
    {
      // Establish a connection to the SMTP server using a SMTPClient:
      client.Connect(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
      // Authenticate the web app with the SMTP server to ensure that only authorized users can send emails through it:
      client.Authenticate(_emailSettings.SmtpUsername, _emailSettings.SmptPassword);
      client.Send(message);
      client.Disconnect(true);
    }





  }
}