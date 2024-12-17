using DevExpress.Xpo;
using InternalWebsite.Application.Utils;
using InternalWebsite.Application.ViewModels;
using InternalWebsite.Core.Entities;
using InternalWebsite.Infrastructure.Data.Context;
using InternalWebsite.Infrastructure.Data.Repositories;
using InternalWebsite.Infrastructure.Data.Services.Interfaces;
using InternalWebsite.ViewModel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services
{
    public class RoleService : GenericRepository<Core.Entities.IdentityRole, RoleDto>, IRoleService
    {
        private readonly IConfiguration _configuration;
        private readonly ResponseHelper _responseHelper;
        private readonly RoleManager<Core.Entities.IdentityRole> _roleManager;

        public RoleService(ClCongDbContext context, IHttpContextAccessor httpContextAccessor,
            ClCongPrincipal clCongPrincipal,
            ResponseHelper responseHelper, RoleManager<Core.Entities.IdentityRole> roleManager,
            IConfiguration configuration) : base(context,
          httpContextAccessor, clCongPrincipal

          )
        {
            _responseHelper = responseHelper;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        public async Task<AppResponse> GetRole()
        {
            try
            {
                var roles = _roleManager.Roles.ToList();
                return new ResponseHelper(_configuration).SuccessMessage(data: roles, message: "Data retrived successfully.");

            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> GetRoleByUser()
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    var roles = _roleManager.Roles.Where(a=>a.CreatedBy == userId).ToList();
                    return new ResponseHelper(_configuration).SuccessMessage(data: roles, message: "Data retrived successfully.");
                }
                return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");

            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> GetRoleById(string id)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (Guid.TryParse(id, out Guid guidValue))
                    {
                        var currentRole = _roleManager.Roles.Where(role => role.Id == guidValue && role.CreatedBy == userId).FirstOrDefault();
                        if (currentRole != null)
                        return new ResponseHelper(_configuration).SuccessMessage(data: currentRole, message: "Data retrived successfully.");
                    }

                    return _responseHelper.SuccessMessage(message: $"No data found against this ID: {id}.");
                }
                return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> CreateRole(RoleDto role)
        {
            try
            {
                if (string.IsNullOrEmpty(role.Name))
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Role name cannot be null or empty.");
                }
                var userId = GetUserDetailId();
                if(userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }
                else
                {
                    var roleCheck = _roleManager.Roles.Where(r => r.Name == role.Name && r.CreatedBy == userId ).FirstOrDefault();

                    if (roleCheck != null)
                    {
                        return new ResponseHelper(_configuration).ErrorMessage(message: "Role already exists.");
                    }
                    IdentityResult roleResult = await _roleManager.CreateAsync(new Core.Entities.IdentityRole { Name = role.Name, CreatedBy = userId,CreatedOn = DateTime.Now });

                    if (roleResult.Succeeded)
                    {
                        return _responseHelper.SuccessMessage(message: "Role Saved Successfully");
                    }
                }
                return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");

            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }

        public async Task<AppResponse> UpdateRole(RoleDto data)
        {

            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (data.Id != "")
                    {
                        if (Guid.TryParse(data.Id, out Guid guidValue)) { 
                            var currentRole = _roleManager.Roles.Where(role => role.Id == guidValue && role.CreatedBy == userId).FirstOrDefault();
                            if (currentRole == null)
                            {
                                return new ResponseHelper(_configuration).ErrorMessage(message: "Role not found against this ID.");
                            }
                            currentRole.Name = data.Name;
                            currentRole.EditBy = userId;
                            currentRole.EditOn = DateTime.Now;
                            await _roleManager.UpdateAsync(currentRole);
                            return _responseHelper.SuccessMessage(message: "Role Updated successfully.");
                        }
                        
                    }
                    return _responseHelper.SuccessMessage(message: $"No data found against this ID: {data.Id}.");

                }
                return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");



            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> DeleteRole(string id)
        {

            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    var role = await _roleManager.FindByIdAsync(id);

                    if (role == null)
                    {
                        return new ResponseHelper(_configuration).ErrorMessage(message: "Role not found.");
                    }
                    IdentityResult roleResult = await _roleManager.DeleteAsync(role);
                    if (roleResult.Succeeded)
                    {
                        return _responseHelper.SuccessMessage(message: "Role deleted successfully.");
                    }
                }
                return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
    }
}
