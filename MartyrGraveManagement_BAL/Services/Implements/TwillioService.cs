using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class TwillioService : ISmsService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromNumber;

        public TwillioService(IConfiguration configuration)
        {
            _accountSid = configuration["Twilio:AccountSid"];
            _authToken = configuration["Twilio:AuthToken"];
            _fromNumber = configuration["Twilio:FromNumber"];
        }

        public async Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                TwilioClient.Init(_accountSid, _authToken);

                var messageResource = await MessageResource.CreateAsync(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(_fromNumber),
                    to: new Twilio.Types.PhoneNumber(phoneNumber)
                );

                return messageResource.Status != MessageResource.StatusEnum.Failed;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }
    }
}
