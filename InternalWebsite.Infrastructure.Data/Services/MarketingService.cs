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
    public class MarketingService : GenericRepository<Core.Entities.IdentityRole, MarketingDto>, IMarketingService
    {
        private readonly IConfiguration _configuration;
        private readonly ResponseHelper _responseHelper;
        private readonly IEmailSenderService _emailSenderService;
        private readonly RoleManager<Core.Entities.IdentityRole> _marketingManager;

        public MarketingService(ClCongDbContext context, IHttpContextAccessor httpContextAccessor,
            ClCongPrincipal clCongPrincipal,
            ResponseHelper responseHelper, RoleManager<Core.Entities.IdentityRole> marketingManager, IEmailSenderService emailSenderService,
            IConfiguration configuration) : base(context,
          httpContextAccessor, clCongPrincipal

          )
        {
            _responseHelper = responseHelper;
            _marketingManager = marketingManager;
            _emailSenderService = emailSenderService;
            _configuration = configuration;
        }
        public async Task<AppResponse> GetMarketing()
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }
                var marketings = _context.Marketings.Where(a=>a.UserId == userId).ToList();
                return new ResponseHelper(_configuration).SuccessMessage(data: marketings, message: "Data retrived successfully.");

            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> GetMarketingByUser()
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    var marketings = _context.Marketings.ToList();
                    return new ResponseHelper(_configuration).SuccessMessage(data: marketings, message: "Data retrived successfully.");
                }
                return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");

            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> GetMarketingById(string id)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (Guid.TryParse(id, out Guid guidValue))
                    {
                        var currentRole = _context.Marketings.Where(marketing => marketing.Id == guidValue).FirstOrDefault();
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
        public async Task<AppResponse> CreateMarketing(MarketingDto marketingModel)
        {
            try
            {
                if (string.IsNullOrEmpty(marketingModel.BusinessName))
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Name cannot be null or empty.");
                }
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }
                else
                {

                    var marketing = new Marketing();
                   
                    marketing.BusinessName = marketingModel.BusinessName;
                    marketing.OwnerName = marketingModel.OwnerName;
                    marketing.PhoneNumber = marketingModel.PhoneNumber;
                    marketing.Objective = marketingModel.Objective;
                    marketing.Duration = marketingModel.Duration;
                    marketing.Type = marketingModel.Type;
                    marketing.Budget = marketingModel.Budget;
                    marketing.Summary = marketingModel.Summary;
                    marketing.Solution = marketingModel.Solution;
                    marketing.Details = marketingModel.Details;
                    marketing.UserId = userId;
                    marketing.IsActive = true;
                    marketing.CreatedOn = DateTime.Now;
                    marketing.CreatedBy = userId;
                    _context.Add(marketing);
                    _context.SaveChanges();
                    var userDetails  = _context.Users.Where(a=>a.Id == userId).FirstOrDefault();
                    if (userDetails != null)
                    {
                        var callbackUrl = "";
                        var emailSendModel = new EmailSendDto()
                        {
                            CallBackUrl = callbackUrl,
                            Email = userDetails.Email,
                            UserName = userDetails.UserName,
                            EmailTitle = "Marketing Service Request",
                            EmailType = "marketing",
                            FirstName = userDetails.FirstName,
                            LastName = userDetails.LastName,
                            OTP = ""
                        };

                        bool isSend = _emailSenderService.MarketingEmailSend(emailSendModel, marketing);
                    }
                    

                    return _responseHelper.SuccessMessage(message: "Saved Successfully");
                }

            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }

        public async Task<AppResponse> UpdateMarketing(MarketingDto data)
        {

            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (data.Id != "")
                    {
                        if (Guid.TryParse(data.Id, out Guid guidValue)) { 
                            var currentMarketing = _context.Marketings.Where(marketing => marketing.Id == guidValue && marketing.UserId == userId).FirstOrDefault();
                            if (currentMarketing == null)
                            {
                                return new ResponseHelper(_configuration).ErrorMessage(message: "Marketing not found against this ID.");
                            }
                            currentMarketing.BusinessName = data.BusinessName;
                            currentMarketing.OwnerName = data.OwnerName;
                            currentMarketing.PhoneNumber = data.PhoneNumber;
                            currentMarketing.Objective = data.Objective;
                            currentMarketing.Duration = data.Duration;
                            currentMarketing.Type = data.Type;
                            currentMarketing.Budget = data.Budget;
                            currentMarketing.Solution = data.Solution;
                            currentMarketing.Details = data.Details;

                            currentMarketing.EditBy = userId;
                            currentMarketing.EditOn = DateTime.Now;
                            _context.SaveChanges();
                            return _responseHelper.SuccessMessage(message: "Updated successfully.");
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
        public async Task<AppResponse> DeleteMarketing(string id)
        {

            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    //var marketing = await _marketingManager.FindByIdAsync(id);

                    //if (marketing == null)
                    //{
                    //    return new ResponseHelper(_configuration).ErrorMessage(message: "Role not found.");
                    //}
                    //IdentityResult marketingResult = await _marketingManager.DeleteAsync(marketing);
                    //if (marketingResult.Succeeded)
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
