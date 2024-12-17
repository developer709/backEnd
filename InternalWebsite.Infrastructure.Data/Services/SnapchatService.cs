using DevExpress.Xpo;
using InternalWebsite.Application.Utils;
using InternalWebsite.Application.ViewModels;
using InternalWebsite.Core.Entities;
using InternalWebsite.Infrastructure.Data.Context;
using InternalWebsite.Infrastructure.Data.Repositories;
using InternalWebsite.Infrastructure.Data.Services.Interfaces;
using InternalWebsite.ViewModel.DTOs;
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
    public class SnapchatService : GenericRepository<Core.Entities.IdentityRole, SnapchatDto>, ISnapchatService
    {
        private readonly IConfiguration _configuration;
        private readonly ResponseHelper _responseHelper;
        private readonly IEmailSenderService _emailSenderService;
        private readonly RoleManager<Core.Entities.IdentityRole> _SnapchatManager;

        public SnapchatService(ClCongDbContext context, IHttpContextAccessor httpContextAccessor,
            ClCongPrincipal clCongPrincipal,
            ResponseHelper responseHelper, RoleManager<Core.Entities.IdentityRole> SnapchatManager, IEmailSenderService emailSenderService,
            IConfiguration configuration) : base(context,
          httpContextAccessor, clCongPrincipal

          )
        {
            _responseHelper = responseHelper;
            _SnapchatManager = SnapchatManager;
            _emailSenderService = emailSenderService;
            _configuration = configuration;
        }
        public async Task<AppResponse> GetSnapchat()
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }
                //var Snapchats = _context.Snapchats.Where(a=>a.UserId == userId).ToList();
                return new ResponseHelper(_configuration).SuccessMessage(data: userId, message: "Data retrived successfully.");

            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> GetSnapchatByUser()
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    //var Snapchats = _context.Snapchats.ToList();
                    return new ResponseHelper(_configuration).SuccessMessage(data: userId, message: "Data retrived successfully.");
                }
                return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");

            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> GetSnapchatById(string id)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    //if (Guid.TryParse(id, out Guid guidValue))
                    //{
                    //    var currentRole = _context.Snapchats.Where(Snapchat => Snapchat.Id == guidValue).FirstOrDefault();
                    //    if (currentRole != null)
                    //    return new ResponseHelper(_configuration).SuccessMessage(data: currentRole, message: "Data retrived successfully.");
                    //}

                    //return _responseHelper.SuccessMessage(message: $"No data found against this ID: {id}.");
                }
                return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> CreateSnapchat(SnapchatDto SnapchatModel)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }
                else
                {

                    return _responseHelper.SuccessMessage(message: "Saved Successfully");
                }

            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }

        public async Task<AppResponse> UpdateSnapchat(SnapchatDto data)
        {

            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    return _responseHelper.SuccessMessage(message: $"No data found against this ID: {data}.");

                }
                return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");



            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> DeleteSnapchat(string id)
        {

            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    //var Snapchat = await _SnapchatManager.FindByIdAsync(id);

                    //if (Snapchat == null)
                    //{
                    //    return new ResponseHelper(_configuration).ErrorMessage(message: "Role not found.");
                    //}
                    //IdentityResult SnapchatResult = await _SnapchatManager.DeleteAsync(Snapchat);
                    //if (SnapchatResult.Succeeded)
                    //{
                    //    return _responseHelper.SuccessMessage(message: "Role deleted successfully.");
                    //}
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
