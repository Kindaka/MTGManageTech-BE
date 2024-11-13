using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface ISmsService
    {
        Task<bool> SendSmsAsync(string phoneNumber, string message);
    }
}
