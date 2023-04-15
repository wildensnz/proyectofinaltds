using InventarioHerramienta.Interfaces;
using InventarioViewModel;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace InventarioHerramienta
{
    public class MailService: IMailService
    {
        private readonly MailSettings _mailSettings;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public Task SendEmailAsync(MailRequest mailRequest)
        {

            try
            {
                var credentials = new NetworkCredential(_mailSettings.Mail,_mailSettings.Password);
                var mail = new MailMessage()
                {
                    From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName),
                    Subject = mailRequest.Subject,
                    Body = mailRequest.Body,
                    IsBodyHtml = true
                };
                mail.To.Add(new MailAddress(mailRequest.ToEmail));
                var client = new SmtpClient()
                {
                    Port = _mailSettings.Port,
                    Host = _mailSettings.Host,
                    EnableSsl = true,
                    Credentials = credentials
                };
                client.Send(mail);
            }
            catch(Exception ex)
            {
                Generic.generateLog(ex.Message, Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Logs", DateTime.Now.ToString("ddMMyyyy") + ".txt"));
                throw new InvalidOperationException(ex.Message);
            }
            return Task.CompletedTask;
        }
    }
}
