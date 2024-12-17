using InternalWebsite.Core.Entities;
using InternalWebsite.ViewModel.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services.Interfaces
{
    public interface IEmailSenderService
    {
        bool SendEmailAsync(string email, string subject, string message);
        bool SendEmailByTemplate(string template, string to);
        bool SendSingUpGroupEmail(EmailSendDto email);
        bool MarketingEmailSend(EmailSendDto email, Marketing marketing);
        bool SendEmailForTimeSheet(string to, string subject);
        bool SendWinEmail(string email, string template);
        //void SendNewWorkerEmailAsync(string email, string subject, string message);
        //void SendNewCompanyEmailAsync(string email, string subject, string message);
        //Task SendEmailConfirmationAsync(string email, string link);
    }
}
