using InternalWebsite.Application.ViewModels;
using InternalWebsite.Core.Entities;
using InternalWebsite.ViewModel.DTOs;
using InternalWebsite.ViewModel.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Repositories.Interfaces
{
    public interface IAccountService
    {
        Task<AppResponse> SignIn(Application.ViewModels.LoginDto user);
        Task<AppResponse> RegisterUser(RegisterDto user);
        Task<AppResponse> GetUserList(string Status);
        Task<AppResponse> UploadProfile(IFormFile file,string fileType);

        Task<AppResponse> CheckUsersDetails(UserDetail userDetail);
        Task<AppResponse> ChangeEmail(ResendEmaillDto user);
        Task<AppResponse> ChangePassword(ChangePassword model);
        Task<AppResponse> GetUserDetailById(long id = 0);
        AppResponse AddPasswordLog(string id, string password);
        Task<AppResponse> LoginUserByCode(Application.ViewModels.LoginbyCodeDto user);
        Task<AppResponse> ConfirmEmail(ConfirmEmailDto model);
        Task<AppResponse> EmailConfirmation(string email, string origin);
        Task<AppResponse> PhoneNumberSend();
        Task<AppResponse> PhoneNumberConfirmation(string code);
        Task<AppResponse> ForgotPassword(PasswordDto model);
        Task<AppResponse> ResetPassword(ResetPasswordlDto model);
        bool ApplicationUserSessionsInsertion(string userId, string token, int LoginStatus, string ipAddress, string userName);
        Task<AppResponse> GetUserDetails();
        Task<AppResponse> GetUserDetailById();
        Task<AppResponse> GetProfileInfoById(string email);
        Task<AppResponse> UploadProfilePic(UploadProfilePic model);
        Task<AppResponse> UpdateProfile(UpdateProfile model);
        Task<AppResponse> UpdateEmail(UpdateProfile model);
        Task<AppResponse> GetCompanyInfo();
        Task<AppResponse> GetUserRole();
        Task<AppResponse> UpdateCompanyInfo(CompanyInfoDto model);
    }
}
