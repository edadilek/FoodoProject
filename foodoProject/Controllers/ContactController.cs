using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

public class ContactController : Controller
{
    [HttpPost]
    public ActionResult SendEmail(string name, string phoneNumber, string email, string subject)
    {
        // Mail ayarları
        var smtpClient = new SmtpClient("smtp-mail.outlook.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(".com", ""),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("edaddd7@hotmail.com"),
            Subject = "Contact Form Submission",
            Body = $"Name: {name}\nPhone: {phoneNumber}\nEmail: {email}\nMessage:\n{subject}",
            IsBodyHtml = false,
        };

        mailMessage.To.Add("edad777@hotmail.com");

        smtpClient.Send(mailMessage);

        return Json(new { success = true });
    }
}
