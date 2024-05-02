using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.BLL.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(MailMessage message);
    }
}
