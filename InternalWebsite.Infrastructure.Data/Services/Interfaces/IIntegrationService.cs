using InternalWebsite.Application.ViewModels;
using InternalWebsite.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services.Interfaces
{
    public interface IIntegrationService
    {
        Task<AppResponse> GetIntegration();
        Task<AppResponse> GetIntegrationByUser();
        Task<AppResponse> GetIntegrationById(string id);
        Task<AppResponse> UpdateIntegration(IntegrationDto data);
        Task<AppResponse> CreateIntegration(IntegrationDto data);
        Task<AppResponse> DeleteIntegration(string id);
    }
}
