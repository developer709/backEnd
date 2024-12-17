using InternalWebsite.Application.ViewModels;
using InternalWebsite.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services.Interfaces
{
    public interface IRoleService
    {
        Task<AppResponse> GetRole();
        Task<AppResponse> GetRoleByUser();
        Task<AppResponse> GetRoleById(string id);
        Task<AppResponse> UpdateRole(RoleDto data);
        Task<AppResponse> CreateRole(RoleDto data);
        Task<AppResponse> DeleteRole(string id);
    }
}
