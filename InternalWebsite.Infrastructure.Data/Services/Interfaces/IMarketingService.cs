using InternalWebsite.Application.ViewModels;
using InternalWebsite.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services.Interfaces
{
    public interface IMarketingService
    {
        Task<AppResponse> GetMarketing();
        Task<AppResponse> GetMarketingByUser();
        Task<AppResponse> GetMarketingById(string id);
        Task<AppResponse> UpdateMarketing(MarketingDto data);
        Task<AppResponse> CreateMarketing(MarketingDto data);
        Task<AppResponse> DeleteMarketing(string id);
    }
}
