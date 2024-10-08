using MartyrGraveManagement_BAL.ModelViews.PaymentDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentDTOResponse> CancelTransaction(PaymentDTORequest paymentRequest);
        Task<PaymentDTOResponse> CreatePayment(PaymentDTORequest paymentRequest);
        Task<List<PaymentDTOResponseForAdmin>> GetPaymentList(DateTime startDate, DateTime endDate, int? status);
    }
}
