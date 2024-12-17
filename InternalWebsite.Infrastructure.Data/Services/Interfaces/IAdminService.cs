using InternalWebsite.Application.ViewModels;
using InternalWebsite.ViewModel.DTOs;
using InternalWebsite.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services.Interfaces
{
    public interface IAdminService
    {
        Task<AppResponse> GetAdminUser();
        Task<AppResponse> GetUserList();
        Task<AppResponse> GetAdminById(string id);
        Task<AppResponse> UpdateAdmin(AdminUser data);
        Task<AppResponse> CreateAdmin(AdminUser data);
        Task<AppResponse> DeleteAdmin(string id);
    }
}
