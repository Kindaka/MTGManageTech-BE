using MartyrGraveManagement_BAL.ModelViews.EmailDTOs;
using MartyrGraveManagement_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface ISendEmailService
    {
        Task SendEmailMartyrGraveAccount(EmailDTO emailDTO);
        string emailBodyForMartyrGraveAccount(Account account, MartyrGrave grave, string randomPassword);
        string emailBodyForUpdateMartyrGraveAccount(Account account, MartyrGrave grave, string randomPassword);
    }
}
