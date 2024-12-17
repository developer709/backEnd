using DevExpress.Xpo;
using InternalWebsite.Application.Utils;
using InternalWebsite.Application.ViewModels;
using InternalWebsite.Core.Entities;
using InternalWebsite.Infrastructure.Data.Context;
using InternalWebsite.Infrastructure.Data.Repositories;
using InternalWebsite.Infrastructure.Data.Services.Interfaces;
using InternalWebsite.ViewModel.DTOs;
using InternalWebsite.ViewModel.Enum;
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
using System.Web;

namespace InternalWebsite.Infrastructure.Data.Services
{
    public class AdminService : GenericRepository<Core.Entities.IdentityRole, AdminDto>, IAdminService
    {
        private readonly IConfiguration _configuration;
        private readonly ResponseHelper _responseHelper;
        private readonly IEmailSenderService _emailSenderService;
        private readonly RoleManager<Core.Entities.IdentityRole> _adminManager;
        private readonly UserManager<tblUser> _userManager;

        public AdminService(ClCongDbContext context, IHttpContextAccessor httpContextAccessor,
            ClCongPrincipal clCongPrincipal,
            UserManager<tblUser> userManager,
            ResponseHelper responseHelper, RoleManager<Core.Entities.IdentityRole> adminManager, IEmailSenderService emailSenderService,
            IConfiguration configuration) : base(context,
          httpContextAccessor, clCongPrincipal

          )
        {
            _responseHelper = responseHelper;
            _userManager = userManager;
            _adminManager = adminManager;
            _emailSenderService = emailSenderService;
            _configuration = configuration;
        }
        public async Task<AppResponse> GetAdminUser()
        {
            try
            {
                var adminRole = await _adminManager.FindByNameAsync("Admin");
                if (adminRole == null)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Admin Role not defined.");
                }

                var adminUsers = await _userManager.Users
                                                         .Where(user => _userManager.GetRolesAsync(user).Result.Any(role => role == "Admin" || role == "SuperAdmin"))
                                                         .ToListAsync();

                var adminUserDtos = new List<AdminUserDto>();

                foreach (var user in adminUsers)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    adminUserDtos.Add(new AdminUserDto
                    {
                        Id = user.Id.ToString(),
                        UserName = user.UserName,
                        Email = user.Email,
                        SignUpDate = user.CreatedOn,
                        LastLogin = user.LastLogin,
                        RoleName = roles.FirstOrDefault() // Add roles to the DTO
                    });
                }



                // Fetch all necessary data from the database first, filtering only CampaignBudgets for admin users
                //var adminUserIds = adminUsers.Select(user => user.Id.ToString()).ToList();
                //var campaignBudgets = _context.CampaignBudgets
                //    .Where(c => adminUserIds.Contains(c.CreatedBy.ToString())) // Filter only those campaigns created by admin users
                //    .ToList();

                //// Perform GroupBy and calculations in memory after retrieving the data
                //var combinedResults = CalculateBudget(campaignBudgets, adminUserDtos);

                return (new ResponseHelper(_configuration).SuccessMessage(data: adminUserDtos));

            }
            catch (Exception e)
            {
                return new ResponseHelper(_configuration).ErrorMessage(e);
            }
        }
        public async Task<AppResponse> GetUserList()
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    // Fetch all users in the Admin role
                    // Fetch all users with Admin or SuperAdmin roles
                    var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
                    var superAdminUsers = await _userManager.GetUsersInRoleAsync("SuperAdmin");

                    // Combine the IDs of Admin and SuperAdmin users
                    var excludedUserIds = adminUsers
                        .Select(user => user.Id.ToString())
                        .Union(superAdminUsers.Select(user => user.Id.ToString()))
                        .ToList();

                    // Fetch all users who are not Admin or SuperAdmin
                    var nonAdminUsers = _context.ApplicationUsers
                        .Where(user => !excludedUserIds.Contains(user.Id.ToString()))
                        .ToList();


                    var nonAdminUserDtos = nonAdminUsers
                        .Select(user => new AdminUserDto
                        {
                            Id = user.Id.ToString(),
                            UserName = user.UserName,
                            Email = user.Email,
                            SignUpDate = user.CreatedOn,
                            Type = user.UserType,
                            LastLogin = user.LastLogin,
                        })
                        .ToList();


                    // Fetch campaign budgets for non-admin users only (exclude campaigns from admin users)
                    var campaignBudgets = _context.CampaignBudgets
                        .Where(c => !excludedUserIds.Contains(c.CreatedBy.ToString())) // Exclude campaigns created by admin users
                        .ToList();

                    var combinedResults = CalculateBudget(campaignBudgets, nonAdminUserDtos);

                    return new ResponseHelper(_configuration).SuccessMessage(data: combinedResults, message: "Data retrived successfully.");
                }
                return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");

            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }

        public List<AdminUserWithCampaignDto> CalculateBudget(List<CampaignBudget> campaignBudgets, List<AdminUserDto> adminUserDtos)
        {
            var today = DateTime.Today;

            var userCampaignSummaries = campaignBudgets
                    .AsEnumerable() // Move to client-side evaluation
                    .GroupBy(c => c.CreatedBy)
                    .Select(g => new UserCampaignSummaryDto
                    {
                        UserId = g.Key.ToString(),
                        TotalBudget = g.Sum(c =>
                            c.ScheduleType == "Instant" ? (decimal)c.Budget :
                            c.ScheduleType == "Custom" && c.StartDate != null && c.EndDate != null ?
                                (today >= c.StartDate && today <= c.EndDate ?
                                    (decimal)(today - c.StartDate.Value).TotalDays * ((decimal)c.Budget / c.NumberOfDay) :
                                    (today > c.EndDate ? (decimal)c.Budget : 0)) : 0
                        ),
                        TotalCampaigns = g.Count()
                    })
                    .ToList();

            // Combine both datasets
            var combinedResults = adminUserDtos
                .GroupJoin(
                    userCampaignSummaries,
                    adminUser => adminUser.Id,
                    campaignSummary => campaignSummary.UserId,
                    (adminUser, campaignSummary) => new AdminUserWithCampaignDto
                    {
                        Id = adminUser.Id,
                        UserName = adminUser.UserName,
                        Email = adminUser.Email,
                        Type = adminUser.Type,
                        SignUpDate = adminUser.SignUpDate,
                        LastLogin = adminUser.LastLogin,
                        TotalBudget = campaignSummary.FirstOrDefault()?.TotalBudget ?? 0,
                        TotalCampaigns = campaignSummary.FirstOrDefault()?.TotalCampaigns ?? 0
                    })
                .ToList();
            return combinedResults;
        }

        public async Task<AppResponse> GetAdminById(string id)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (Guid.TryParse(id, out Guid guidValue))
                    {
                        var currentUser = _context.ApplicationUsers.FirstOrDefault(admin => admin.Id == guidValue);
                        var getCompanyInfo = _context.CompanyInfoes.FirstOrDefault(a => a.UserId == guidValue);

                        if (currentUser == null)
                        {
                            return _responseHelper.ErrorMessage("User information not found.");
                        }
                        // Fetch user roles
                        var userIdentity = await _userManager.FindByIdAsync(currentUser.Id.ToString());
                        if (userIdentity == null)
                        {
                            return _responseHelper.ErrorMessage("User identity not found.");
                        }
                        var roles = await _userManager.GetRolesAsync(userIdentity);
                        var userCompanyInfoDto = new UserCompanyInfoDto
                        {
                            FirstName = currentUser.FirstName,
                            LastName = currentUser.LastName,
                            UserType = currentUser.UserType,
                            Picture = currentUser.Picture,
                            StreetAddress1 = currentUser.StreetAddress1,
                            StreetAddress2 = currentUser.StreetAddress2,
                            City = currentUser.City,
                            State = currentUser.State,
                            ZipCode = currentUser.ZipCode,
                            PhoneNumber = currentUser.PhoneNumber,
                            Email = currentUser.Email,
                            Country = currentUser.Country,
                            Id = currentUser.Id.ToString(),
                            Name = getCompanyInfo != null ? getCompanyInfo.Name ?? "" : "",
                            VatNumber = getCompanyInfo != null ? getCompanyInfo.VatNumber ?? "" : "",
                        };
                        if (roles.Count > 0)
                            userCompanyInfoDto.Roles = roles.ToList();

                        if (userCompanyInfoDto != null)
                            return new ResponseHelper(_configuration).SuccessMessage(data: userCompanyInfoDto, message: "Data retrived successfully.");
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
        public async Task<AppResponse> UpdateAdmin(AdminUser model)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }
                if (Guid.TryParse(model.Id, out Guid guidValue))
                {
                    var userData = _context.ApplicationUsers.Where(a => a.Id == guidValue).FirstOrDefault();
                    if (userData == null)
                    {
                        return (new ResponseHelper(_configuration).ErrorMessage(message: "user not found"));
                    }

                    userData.FirstName = model.FirstName;
                    userData.LastName = model.LastName;
                    userData.Picture = model.Picture;
                    userData.PhoneNumber = model.PhoneNumber;
                    userData.StreetAddress1 = model.StreetAddress1;
                    userData.StreetAddress2 = model.StreetAddress2;
                    userData.City = model.City;
                    userData.State = model.State;
                    userData.ZipCode = model.ZipCode;
                    userData.Country = model.Country;
                    userData.EditOn = DateTime.Now;
                    userData.EditBy = userId;

                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(userData);
                        var result = await _userManager.ResetPasswordAsync(userData, token, model.Password);
                        if (result.Succeeded)
                        {
                            _context.SaveChanges();
                        }
                    }
                    if (!string.IsNullOrEmpty(model.UserType))
                    {
                        var roleResult = await _userManager.AddToRoleAsync(userData, model.UserType);
                        if (!roleResult.Succeeded)
                        {
                            var roleErrorMessages = roleResult.Errors.Select(e => e.Description).ToList();
                            return _responseHelper.ErrorMessage(message: "User created, but failed to add to assign role: " + string.Join("; ", roleErrorMessages));
                        }
                    }

                    var getcompanyInfo = _context.CompanyInfoes.Where(a => a.UserId == guidValue).FirstOrDefault();
                    if (getcompanyInfo == null)
                    {
                        var company = new CompanyInfo();
                        company.UserId = guidValue;
                        company.Name = model.Name;
                        company.VatNumber = model.VatNumber;
                        company.IsActive = true;
                        company.CreatedOn = DateTime.Now;
                        company.CreatedBy = userId;
                        _context.Add(company);
                    }
                    else
                    {
                        getcompanyInfo.Name = model.Name;
                        getcompanyInfo.VatNumber = model.VatNumber;
                        getcompanyInfo.EditOn = DateTime.Now;
                        getcompanyInfo.EditBy = userId;
                    }
                    _context.SaveChanges();
                    return _responseHelper.SuccessMessage(message: "Saved Successfully");
                }

                return _responseHelper.ErrorMessage(message: "Provide user Id");

            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }

        public async Task<AppResponse> CreateAdmin(AdminUser model)
        {

            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (string.IsNullOrEmpty(model.Email))
                    {
                        return _responseHelper.ErrorMessage(message: "Please enter a valid email address");
                    }

                    var emailExists = _context.ApplicationUsers.Where(a => a.Email.ToLower() == model.Email.ToLower()).FirstOrDefault();
                    if (emailExists != null)
                    {
                        return _responseHelper.ErrorMessage(message: "Email already exists. Please enter a valid email address");
                    }
                    if (!string.IsNullOrEmpty(model.UserType))
                    {
                        model.Password = "Pa$$w0rd!";
                    }
                    var newUser = new tblUser
                    {
                        Email = model.Email,
                        UserName = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        PhoneNumber = model.PhoneNumber,
                        EmailConfirmed = true,
                        Picture = model.Picture,
                        PhoneNumberConfirmed = true,
                        StreetAddress1 = model.StreetAddress1,
                        StreetAddress2 = model.StreetAddress2,
                        City = model.City,
                        State = model.State,
                        ZipCode = model.ZipCode,
                        Country = model.Country,
                        CreatedOn = DateTime.Now,
                        CreatedBy = userId,
                    };
                    var result = await _userManager.CreateAsync(newUser, model.Password);
                    if (!result.Succeeded)
                    {
                        var errorMessages = result.Errors.Select(e => e.Description).ToList();
                        return _responseHelper.ErrorMessage(message: string.Join("; ", errorMessages));
                    }
                    // Add the new user to the "Admin" role
                    if (!string.IsNullOrEmpty(model.UserType))
                    {
                        var roleResult = await _userManager.AddToRoleAsync(newUser, model.UserType);
                        if (!roleResult.Succeeded)
                        {
                            var roleErrorMessages = roleResult.Errors.Select(e => e.Description).ToList();
                            return _responseHelper.ErrorMessage(message: "User created, but failed to add to assign role: " + string.Join("; ", roleErrorMessages));
                        }
                        var emailDto = new EmailSendDto() { Email = newUser.Email, FirstName = newUser.FirstName, LastName = newUser.LastName, UserName = newUser.UserName, UserId = newUser.Id };
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                        var sendPassword = GenerateRandomCode();

                        var emailType = EmailAction.ResetPassword;
                        AppUserVerification verfication = new AppUserVerification();
                        verfication.ApplicationUserId = newUser.Id;
                        verfication.VerificationCode = sendPassword;
                        verfication.VerificationToken = token;
                        verfication.Email = newUser.Email;
                        verfication.IsValid = true;
                        verfication.Type = emailType;
                        verfication.ExpiryDate = DateTime.Now.AddDays(7);
                        verfication.CreatedDate = DateTime.Now;
                        verfication.CreatedById = 0;
                        _context.AppUserVerifications.Add(verfication);
                        _context.SaveChanges();

                        var url =
                            $"{_httpContextAccessor?.HttpContext?.Request.Scheme}://{_httpContextAccessor?.HttpContext?.Request.Host}";
                        var encodedToken = HttpUtility.UrlEncode(token);
                        var callbackUrl = _configuration["Target:Admin_Url_Prod"].ToString() + "auth/set-password?code=" +
                                          encodedToken +
                                          "&email=" + newUser.Email;
                        var emailSendModel = new EmailSendDto() { CallBackUrl = callbackUrl, Email = newUser.Email, UserName = newUser.UserName, EmailTitle = "Password Set", EmailType = "passwordset", FirstName = newUser.FirstName, LastName = newUser.LastName, OTP = sendPassword };

                        bool isSend = _emailSenderService.SendSingUpGroupEmail(emailSendModel);

                    }

                    var company = new CompanyInfo();
                    company.UserId = newUser.Id;
                    company.Name = model.Name;
                    company.VatNumber = model.VatNumber;
                    company.IsActive = true;
                    company.CreatedOn = DateTime.Now;
                    company.CreatedBy = userId;
                    _context.Add(company);

                    _context.SaveChanges();


                    return _responseHelper.SuccessMessage("User created and added to Admin role successfully.");

                }
                return new ResponseHelper(_configuration).ErrorMessage(message: "No authorized user");
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public static string GenerateRandomCode(int length = 6)
        {
            Random random = new Random();
            string code = "";

            for (int i = 0; i < length; i++)
            {
                code += random.Next(0, 10).ToString();
            }

            return code;
        }
        public async Task<AppResponse> DeleteAdmin(string id)
        {

            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    //var admin = await _adminManager.FindByIdAsync(id);

                    //if (admin == null)
                    //{
                    //    return new ResponseHelper(_configuration).ErrorMessage(message: "Role not found.");
                    //}
                    //IdentityResult adminResult = await _adminManager.DeleteAsync(admin);
                    //if (adminResult.Succeeded)
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
