using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DemoWebsite.Models
{
    public class SmtpConfig
    {        
        public string Host { get; set; }
        public int Port { get; set; }
        public string From { get; set; }
        public NetworkCredential Credentials { get; set; }
        public bool EnableSsl { get; set; }
    }

    public class MailingService
    {
        private SmtpConfig smtpConfig;
        private SmtpClient smtpClient;

        public MailingService(IOptions<SmtpConfig> SmtpConfig)
        {
            smtpConfig = SmtpConfig.Value;
            smtpClient = new SmtpClient();
            smtpClient.Host = smtpConfig.Host;
            smtpClient.Port = smtpConfig.Port;
            smtpClient.Credentials = smtpConfig.Credentials;            
            smtpClient.EnableSsl = smtpConfig.EnableSsl;
        }

        public async void SendEmail(string emailTo, string subject, string body)
        {            
            var message = new MailMessage();
            message.To.Add(new MailAddress(emailTo)); 
            message.From = new MailAddress(smtpConfig.From);
            message.Subject = subject;
            message.Body = string.Format(body);
            message.IsBodyHtml = true;

            using (var smtp = smtpClient)
            {
                await smtp.SendMailAsync(message);                
            }            
        }
    }
}
