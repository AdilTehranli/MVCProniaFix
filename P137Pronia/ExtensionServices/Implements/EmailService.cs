using P137Pronia.ExtensionServices.Interfaces;
using System.Net;
using System.Net.Mail;

namespace P137Pronia.ExtensionServices.Implements;

public class EmailService : IEmailService
{
    readonly IConfiguration _configration;

    public EmailService(IConfiguration configration)
    {
        _configration = configration;
    }

    public void send(string toMail, string subject, string message, bool isBodyHtml = true)
    {
        SmtpClient smpt = new SmtpClient();
        smpt.Port = Convert.ToInt32(_configration["Email:Port"]);
        smpt.Host = _configration["Email:Host"];
        smpt.EnableSsl = true;

        MailAddress from = new MailAddress(_configration["Email:Username"], "Pronia support");
        MailAddress to = new MailAddress(toMail);
        NetworkCredential networ = new NetworkCredential(_configration["Email:Username"], _configration["Email:Password"]);
        smpt.Credentials = networ;

        MailMessage mm = new MailMessage(from,to);
        mm.Subject = subject;
        mm.Body = message;
        mm.IsBodyHtml = isBodyHtml;
        smpt.Send(mm);
    }
}
