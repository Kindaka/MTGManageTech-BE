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
using System.Diagnostics;

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
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Thông báo tài khoản</title>
    <style>
        body {{
            font-family: 'Arial', sans-serif;
            background-color: #f4f4f9;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 500px;
            margin: 0 auto;
            padding: 10px;
            background-color: #ffffff;
            border-radius: 12px;
            box-shadow: 0 0 15px rgba(0, 0, 0, 0.1);
            border: 1px solid #dcdcdc;
        }}
        .header {{
            text-align: center;
            padding: 20px;
            background-color: #82613b; /* Màu nâu nhạt */
            color: #ffffff; /* Chữ màu trắng */
            border-radius: 12px 12px 0 0;
        }}
        .header img {{
            max-width: 100px;
            height: auto;
            margin-bottom: 10px;
        }}
        .header h1 {{
            margin: 0;
            font-size: 26px;
            font-weight: bold;
        }}
        .content {{
            padding: 10px;
            color: #333;
            text-align: left;
            line-height: 1.6;
        }}
        .content p {{
            font-size: 16px;
            color: #555;
            white-space: normal;
            text-overflow: clip;
            overflow: visible;
        }}
        .content .details {{
            background-color: #E8F5E9; /* Màu nền nhạt hài hòa */
            padding: 15px;
            border-left: 4px solid #82613b; /* Viền màu nâu */
            border-radius: 8px;
            margin: 20px 0;
        }}
        .details p {{
            margin: 0 0 5px;
            font-size: 15px;
            color: #333;
        }}
        .details strong {{
            color: #333;
        }}
        .footer {{
            margin-top: 10px;
            padding: 10px;
            background-color: #82613b; /* Màu nâu nhạt */
            color: #ffffff; /* Chữ màu trắng */
            text-align: center;
            border-radius: 0 0 12px 12px;
        }}
        .footer p {{
            margin: 0;
            font-size: 14px;
            line-height: 1.5;
        }}
        .footer a {{
            color: #ffffff;
            text-decoration: none;
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <img src='https://annhienedu.vn/wp-content/uploads/2023/07/logo-giao-duc-an-nhien.png' alt='An Nhiên Logo'>
            <h1>Thông báo tài khoản An Nhiên</h1>
        </div>
        <div class='content'>
            <p>Xin chào người dùng {account.FullName},</p>
            <p>Nghĩa trang đã cập nhật mộ liệt sĩ vào hệ thống của An Nhiên. Bạn sẽ được cấp tài khoản để sử dụng phần mềm của chúng tôi.</p>

            <div class='details'>
                <p><strong>Tên mộ liệt sĩ:</strong> {grave.MartyrGraveInformations.FirstOrDefault()?.Name}</p>
                <p><strong>Vị trí mộ:</strong> {grave.MartyrCode}</p>
                <p><strong>Tên tài khoản đăng nhập:</strong> {account.PhoneNumber}</p>        
                <p><strong>Mật khẩu:</strong> {randomPassword}</p>
            </div>

            <p><strong>Lưu ý:</strong> Khi đăng nhập lần đầu, bạn cần đổi mật khẩu để bảo mật tài khoản và sử dụng các dịch vụ trong mục hồ sơ khách hàng.</p>
        </div>
        <div class='footer'>
            <p>Trân trọng,<br/>Đội ngũ Admin từ Nghĩa trang An Nhiên</p>
            <p><a href='https://annhien.vn'>Truy cập trang web của chúng tôi</a></p>
        </div>
    </div>
</body>
</html>
";
        }

        public string emailBodyForUpdateMartyrGraveAccount(Account account, MartyrGrave grave, string randomPassword)
        {
            return $@"
<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Thông báo tài khoản</title>
    <style>
        body {{
            font-family: 'Arial', sans-serif;
            background-color: #f4f4f9;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 500px;
            margin: 0 auto;
            padding: 10px;
            background-color: #ffffff;
            border-radius: 12px;
            box-shadow: 0 0 15px rgba(0, 0, 0, 0.1);
            border: 1px solid #dcdcdc;
        }}
        .header {{
            text-align: center;
            padding: 20px;
            background-color: #82613b; /* Màu nâu nhạt */
            color: #ffffff; /* Chữ màu trắng */
            border-radius: 12px 12px 0 0;
        }}
        .header img {{
            max-width: 100px;
            height: auto;
            margin-bottom: 10px;
        }}
        .header h1 {{
            margin: 0;
            font-size: 26px;
            font-weight: bold;
        }}
        .content {{
            padding: 10px;
            color: #333;
            text-align: left;
            line-height: 1.6;
        }}
        .content p {{
            font-size: 16px;
            color: #555;
            white-space: normal;
            text-overflow: clip;
            overflow: visible;
        }}
        .content .details {{
            background-color: #E8F5E9; /* Màu nền nhạt hài hòa */
            padding: 15px;
            border-left: 4px solid #82613b; /* Viền màu nâu */
            border-radius: 8px;
            margin: 20px 0;
        }}
        .details p {{
            margin: 0 0 5px;
            font-size: 15px;
            color: #333;
        }}
        .details strong {{
            color: #333;
        }}
        .footer {{
            margin-top: 10px;
            padding: 10px;
            background-color: #82613b; /* Màu nâu nhạt */
            color: #ffffff; /* Chữ màu trắng */
            text-align: center;
            border-radius: 0 0 12px 12px;
        }}
        .footer p {{
            margin: 0;
            font-size: 14px;
            line-height: 1.5;
        }}
        .footer a {{
            color: #ffffff;
            text-decoration: none;
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <img src='https://annhienedu.vn/wp-content/uploads/2023/07/logo-giao-duc-an-nhien.png' alt='An Nhiên Logo'>
            <h1>Thông báo tài khoản An Nhiên</h1>
        </div>
        <div class='content'>
            <p>Xin chào người dùng {account.FullName},</p>
            <p>Nghĩa trang đã cập nhật tài khoản của bạn vào mộ thân nhân trong hệ thống của An Nhiên. Bạn sẽ được cấp tài khoản để sử dụng phần mềm của chúng tôi.</p>

            <div class='details'>
                <p><strong>Tên tài khoản đăng nhập:</strong> {account.PhoneNumber}</p>        
                <p><strong>Mật khẩu:</strong> {randomPassword}</p>
            </div>

            <p><strong>Lưu ý:</strong> Khi đăng nhập lần đầu, bạn cần đổi mật khẩu để bảo mật tài khoản và sử dụng các dịch vụ trong mục hồ sơ khách hàng.</p>
        </div>
        <div class='footer'>
            <p>Trân trọng,<br/>Đội ngũ Admin từ Nghĩa trang An Nhiên</p>
            <p><a href='https://annhien.vn'>Truy cập trang web của chúng tôi</a></p>
        </div>
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
