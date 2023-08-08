namespace P137Pronia.ExtensionServices.Interfaces;

public interface IEmailService
{
    void send(string toMail,string subject,string message,bool isBodyHtml=true);
}
