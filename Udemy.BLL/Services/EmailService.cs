
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Udemy.BLL.Services.Interfaces;
using Udemy.CU.Helper;

namespace Udemy.BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
            _smtpClient = new SmtpClient(_smtpSettings.Server)
            {
                
                Port = _smtpSettings.Port,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = true
            };
        }

        public async Task SendEmailAsync(MailMessage message)
        {
            await _smtpClient.SendMailAsync(message);
        }
    }
}
