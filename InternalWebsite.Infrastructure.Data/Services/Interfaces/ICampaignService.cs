using InternalWebsite.Application.ViewModels;
using InternalWebsite.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services.Interfaces
{
    public interface ICampaignService
    {
        Task<AppResponse> GetCampaign();
        Task<AppResponse> GetCampaignByUser();
        Task<AppResponse> GetCampaignById(string id);
        Task<AppResponse> GetCampaignDetails(string id);
        Task<AppResponse> UpdateCampaign(CampaignDto data);
        Task<AppResponse> CreateCampaign(CampaignDto data);
        Task<AppResponse> DeleteCampaign(string id);
        Task<AppResponse> GetCampaignBudgetById(string id);
        Task<AppResponse> GetCampaignAudienceById(string id);
        Task<AppResponse> GetCampaignContentById(string id);
        Task<AppResponse> CampaignStart(string id);
        Task<AppResponse> CreateUpdateBudget(CampaignBudgetDto data);
        Task<AppResponse> CreateUpdateAudience(CampaignAudienceDto data);
        Task<AppResponse> CreateUpdateContent(CampaignContentDto data);
        Task<AppResponse> CampaignStatus(CampaignStatusDto data);
    }
}
