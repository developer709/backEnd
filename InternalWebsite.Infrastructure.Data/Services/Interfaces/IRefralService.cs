using InternalWebsite.Application.ViewModels;
using InternalWebsite.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services.Interfaces
{
    public interface IRefralService
    {
        Task<AppResponse> GetRefrals();
        Task<AppResponse> GetRefralById(string id);
        Task<AppResponse> GetRefralSByRefranceId(string id);
        Task<AppResponse> CreateRefral(RefralDto data);
        Task<AppResponse> UpdateRefral(RefralDto data);
        Task<AppResponse> UpdateRefralByUser();
        Task<AppResponse> DeleteRefral(string id);
    }
}
