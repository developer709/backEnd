using InternalWebsite.Application.ViewModels;
using InternalWebsite.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services.Interfaces
{
    public interface ILookupService
    {
        Task<AppResponse> GetLookup();
        Task<AppResponse> GetLookupById(string id);
        Task<AppResponse> GetLookupByType(string type);
        Task<AppResponse> CreateLookup(LookupDto data);
        Task<AppResponse> UpdateLookup(LookupDto data);
        Task<AppResponse> DeleteLookup(string id);

    }
}
