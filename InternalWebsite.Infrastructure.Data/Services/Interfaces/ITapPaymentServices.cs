using InternalWebsite.Application.ViewModels;
using InternalWebsite.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services.Interfaces
{
    public interface ITapPaymentServices
    {
        Task<AppResponse> GetWallet();
        Task<AppResponse> GetReferEarn();
        Task<AppResponse> RetrieveAuthorize(string id);
        Task<AppResponse> CreateAuthorize(PaymentRequestDto data);
        Task<AppResponse> CreateCharge(PaymentRequestDto data);
        Task<AppResponse> RetrieveCharge(string id);
        Task<AppResponse> RetrieveChargeList(ChargeDto requestList);
        Task<AppResponse> DownloadCharge(ChargeDto requestList);
        Task<AppResponse> CreateRefund(RefundDto requestData);
    }
}
