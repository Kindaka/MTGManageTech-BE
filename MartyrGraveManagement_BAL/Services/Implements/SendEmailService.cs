using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.EmailDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using MartyrGraveManagement_DAL.Entities;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class SendEmailService : ISendEmailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public SendEmailService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public string emailBodyForMartyrGraveAccount(Account account, MartyrGrave grave, string randomPassword)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Email Notification</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 5px;
            background-color: #f9f9f9;
        }}
        h1 {{
            color: #333;
        }}
        p {{
            color: #666;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>Tài khoản đăng nhập vào phần mềm An Nhiên</h1>
        <p>Gửi {account.FullName},</p>
        <p>Nghĩa trang đã cập nhật mộ liệt sĩ vào trong hệ thống của An Nhiên, với diện là đại diện thân nhân bạn sẽ được cấp tài khoản để sử dụng phần mềm của chúng tôi</p>        
        <p>Tên mộ liệt sĩ: {grave.MartyrGraveInformations.FirstOrDefault()?.Name}</p>
        <p>Vị trí mộ: {grave.MartyrCode}</p>
        <p>Tên tài khoản đăng nhập: {account.AccountName}</p>        
        <p>Mật khẩu: {randomPassword}</p>

        <p>Lưu ý: Khi đăng nhập bạn cần đổi mật khẩu để bảo mật tài khoản của mình cho việc sử dụng dịch vụ ở mục hồ sơ khách hàng</p></br></br>

        <p>Thân ái,<br/>[Admin từ Nghĩa trang An Nhiên]</p>
    </div>
</body>
</html>
";
        }

        public async Task SendEmailMartyrGraveAccount(EmailDTO emailDTO)
        {
            try
            {
                string fromMail = _config.GetSection("EmailUsername").Value;
                string fromPassword = _config.GetSection("EmailPassword").Value;

                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromMail);
                message.Subject = emailDTO.Subject;
                message.To.Add(new MailAddress(emailDTO.To));
                message.Body = emailDTO.Body;
                message.IsBodyHtml = true;

                var smtpClient = new SmtpClient(_config.GetSection("EmailHost").Value)
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromMail, fromPassword),
                    EnableSsl = true,
                };

                smtpClient.Send(message);
            }
            catch
            {
                throw new Exception("Error while send mail");
            }
        }
    }
}
