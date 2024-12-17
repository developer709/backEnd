using InternalWebsite.Application.ViewModels;
using InternalWebsite.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<AppResponse> GetUserCount(string filter);
        Task<AppResponse> GetCamapignTotal(string filter);
        Task<AppResponse> TotalBudget(string filter);
    }
}
