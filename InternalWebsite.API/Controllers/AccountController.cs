using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using InternalWebsite.Application.Utils;
using InternalWebsite.Application.ViewModels;
using InternalWebsite.Core.Entities;
using InternalWebsite.API.Extensions;
using InternalWebsite.Infrastructure.Data.Repositories.Interfaces;
using InternalWebsite.Infrastructure.Data.Services;
using InternalWebsite.Infrastructure.Data.Services.Interfaces;
using InternalWebsite.ViewModel.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using InternalWebsite.ViewModel.Models;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace InternalWebsite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ClCongController
    {
        private readonly IAccountService _accountService;
        public AccountController(ILogger<AccountController> logger,
            UserManager<tblUser> userManager,
            IConfiguration config, IAccountService accountService) : base(logger, config, userManager)
        {
            _accountService = accountService;
        }
        [HttpGet]
        [Route("GetUserDetails")]
        public async Task<IActionResult> GetUserDetails()
        {
            try
            {
                var resp = await _accountService.GetUserDetails();
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());
                return Ok(new ResponseHelper(Config).ErrorMessage(ex));
            }
        }

        [HttpGet]
        [Route("GetUserList")]
        public async Task<IActionResult> GetUserList(string status)
        {
            try
            {
                var resp = await _accountService.GetUserList(status);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());
                return Ok(new ResponseHelper(Config).ErrorMessage(ex));
            }
        }
        [HttpGet]
        [Route("GetUserDetailById")]
        public async Task<IActionResult> GetUserDetailById()
        {
            try
            {
                var resp = await _accountService.GetUserDetailById();
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());
                return Ok(new ResponseHelper(Config).ErrorMessage(ex));
            }
        }

        [HttpGet]
        [Authorize]
        [Route("GetProfileInfoById")]
        public async Task<IActionResult> GetProfileInfoById(string email)
        {
            try
            {
                var resp = await _accountService.GetProfileInfoById(email);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());
                return Ok(new ResponseHelper(Config).ErrorMessage(ex));
            }
        }
        [HttpGet]
        [Route("VerifyEmailExists")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string key)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    throw new NullReferenceException("Invalid Email Provided!");

                if (string.IsNullOrEmpty(key))
                    return Ok(new ResponseHelper(Config).ErrorMessage(message: "Email Already Exist!"));

                var user = await UserManager.FindByEmailAsync(email);
                return Ok(user == null
                    ? new ResponseHelper(Config).SuccessMessage(message: "Email Doesn't Exist!", data: false,
                        miscData: null)
                    : new ResponseHelper(Config).ErrorMessage(message: "Email Already Exist!"));
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());

                return Ok(new ResponseHelper(Config).ErrorMessage(ex));
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("CheckUsersDetails")]
        public async Task<IActionResult> CheckUsersDetails(UserDetail userDetail)
        {
            if (!ModelState.IsValid)
                return Ok(new ResponseHelper(Config).ErrorMessage(message: "Invalid Object"));
            try
            {
                if (string.IsNullOrEmpty(userDetail.Email) || string.IsNullOrWhiteSpace(userDetail.Email))
                    return Ok(new ResponseHelper(Config).ErrorMessage(message: "The Email field is required."));
                if (string.IsNullOrEmpty(userDetail.Password) || string.IsNullOrWhiteSpace(userDetail.Password))
                    return Ok(new ResponseHelper(Config).ErrorMessage(message: "The Password field is required."));

                var resp = await _accountService.CheckUsersDetails(userDetail);


                return Ok(resp);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] Application.ViewModels.LoginDto model)
        {
            if (!ModelState.IsValid)
                return Ok(new ResponseHelper(Config).ErrorMessage(message: "Invalid Object"));
            try
            {
                
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrWhiteSpace(model.Email))
                    return Ok(new ResponseHelper(Config).ErrorMessage(message: "The Email field is required."));
                if (string.IsNullOrEmpty(model.UserType) || !model.UserType.Equals("google", StringComparison.OrdinalIgnoreCase))
                {
                    //var validator = new ReCaptchaValidator("6Le_dSoqAAAAANeamiOr23J1PLFf8sixrimvwGw6");
                    //if (!await validator.IsValid(model.GRecaptchaResponse))
                    //{
                    //    // Handle reCAPTCHA validation failure
                    //    return Ok(new ResponseHelper(Config).ErrorMessage(message: "Please verify that you are not a robot."));
                    //}
                    if (string.IsNullOrEmpty(model.Password) || string.IsNullOrWhiteSpace(model.Password))
                        return Ok(new ResponseHelper(Config).ErrorMessage(message: "The Password field is required."));
                }
                var referer = Request.Headers["Referer"].ToString();
                var origin = Request.Headers["Origin"].ToString();
                model.Origin = origin;
                var resp = await _accountService.SignIn(model);


                return Ok(resp);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return Ok(new ResponseHelper(Config).ErrorMessage(message: "Invalid Object"));
            try
            {
                var referer = Request.Headers["Referer"].ToString();
                var origin = Request.Headers["Origin"].ToString();
                model.Origin = origin;
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrWhiteSpace(model.Email))
                    return Ok(new ResponseHelper(Config).ErrorMessage(message: "The Email field is required."));
                if (string.IsNullOrEmpty(model.UserType) || !model.UserType.Equals("google", StringComparison.OrdinalIgnoreCase))
                {
                    //var validator = new ReCaptchaValidator("6Le_dSoqAAAAANeamiOr23J1PLFf8sixrimvwGw6");
                    //if (!await validator.IsValid(model.GRecaptchaResponse))
                    //{
                    //    // Handle reCAPTCHA validation failure
                    //    return Ok(new ResponseHelper(Config).ErrorMessage(message: "Please verify that you are not a robot."));
                    //}
                    if (string.IsNullOrEmpty(model.Password) || string.IsNullOrWhiteSpace(model.Password))
                        return Ok(new ResponseHelper(Config).ErrorMessage(message: "The Password field is required."));
                }

                var ss = await _accountService.RegisterUser(model);
                return Ok(ss);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());

                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto model)
        {
            try
            {
                var resp = await _accountService.ConfirmEmail(model);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());

                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        [Authorize]
        [HttpPost]
        [Route("EmailConfirmation")]
        public async Task<IActionResult> EmailConfirmation(string email)
        {
            try
            {
                var referer = Request.Headers["Referer"].ToString();
                var origin = Request.Headers["Origin"].ToString();
                var resp = await _accountService.EmailConfirmation(email,origin);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());

                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        [Authorize]
        [HttpPost]
        [Route("PhoneNumberSend")]
        public async Task<IActionResult> PhoneNumberSend()
        {
            try
            {
                var resp = await _accountService.PhoneNumberSend();
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());

                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        [Authorize]
        [HttpPost]
        [Route("PhoneNumberConfirmation")]
        public async Task<IActionResult> PhoneNumberConfirmation(string code)
        {
            try
            {
                var resp = await _accountService.PhoneNumberConfirmation(code);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());

                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(PasswordDto model)
        {
            try
            {
                // Get the 'Referer' or 'Origin' header from the request
                var referer = Request.Headers["Referer"].ToString();
                var origin = Request.Headers["Origin"].ToString();
                var validator = new ReCaptchaValidator("6Le_dSoqAAAAANeamiOr23J1PLFf8sixrimvwGw6");
                if (!await validator.IsValid(model.GRecaptchaResponse))
                {
                    // Handle reCAPTCHA validation failure
                    return Ok(new ResponseHelper(Config).ErrorMessage(message: "Please verify that you are not a robot."));
                }
                model.Origin = origin;
                var resp = await _accountService.ForgotPassword(model);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());

                return Ok(new ResponseHelper(Config).ErrorMessage(message: ex.ToString()));
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordlDto modal)
        {
            try
            {
                var validator = new ReCaptchaValidator("6Le_dSoqAAAAANeamiOr23J1PLFf8sixrimvwGw6");
                if (!await validator.IsValid(modal.GRecaptchaResponse))
                {
                    // Handle reCAPTCHA validation failure
                    return Ok(new ResponseHelper(Config).ErrorMessage(message: "Please verify that you are not a robot."));
                }
                var resp = await _accountService.ResetPassword(modal);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ResendEmail")]
        public async Task<IActionResult> ResendEmail(ResendEmaillDto model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrWhiteSpace(model.Email))
                    return Ok(new ResponseHelper(Config).ErrorMessage(message: "The Email field is required."));

                var ss = await _accountService.ChangeEmail(model);
                return Ok(ss);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());

                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        [HttpPost]
        [Authorize]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePassword model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.OldPassword) || string.IsNullOrWhiteSpace(model.NewPassword))
                    return Ok(new ResponseHelper(Config).ErrorMessage(message: "The Password field is required."));

                var ss = await _accountService.ChangePassword(model);
                return Ok(ss);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());

                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        [HttpPost]
        [Authorize]
        [Route("LogOut")]
        public async Task<IActionResult> logOut(string token, string IPAddress)
        {
            try
            {

                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
                var user = User as ClaimsPrincipal;
                var identity = user.Identity as ClaimsIdentity;
                var claimNameList = identity.Claims.Select(x => x.Type).ToList();
                var userId = identity.Claims.Where(a => a.Type == "UserId").Select(a => a.Value).FirstOrDefault();
                var email = identity.Claims.Where(a => a.Type == "Email").Select(a => a.Value).FirstOrDefault();

                foreach (var name in claimNameList)
                {

                    //if (name.Equals("UserId") || name.Equals("Id"))
                    //{
                    //    var claim = identity.Claims.FirstOrDefault(x => x.Type == name);
                    //    if (claim != null)
                    //        identity.RemoveClaim(claim);
                    //}
                    var claim = identity.Claims.FirstOrDefault(x => x.Type == name);
                    if (claim != null)
                        identity.RemoveClaim(claim);
                }


                return Ok(new ResponseHelper(Config).SuccessMessage());
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());

                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        [Route("Upload")]

        [HttpPost, DisableRequestSizeLimit]
        public IActionResult Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                try
                {
                    return Ok(_accountService.UploadProfile(file,"Images"));
                }
                catch (Exception ex)
                {
                    return Ok(new ResponseHelper(Config).ErrorMessage(ex.Message + Environment.NewLine + ex.StackTrace));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        [Route("UploadVideo")]
        [HttpPost, DisableRequestSizeLimit]
        public IActionResult UploadVideo()
        {
            try
            {
                var file = Request.Form.Files[0];

                // Maximum allowed file size (16 MB)
                const long MaxFileSize = 16 * 1024 * 1024; // 16 MB in bytes

                if (file.Length > MaxFileSize)
                {
                    return Ok(new ResponseHelper(Config).ErrorMessage("File size exceeds the 16 MB limit."));
                }

                try
                {
                    return Ok(_accountService.UploadProfile(file,"Videos"));
                }
                catch (Exception ex)
                {
                    return Ok(new ResponseHelper(Config).ErrorMessage(ex.Message + Environment.NewLine + ex.StackTrace));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost]
        [Authorize]
        [Route("UploadProfilePic")]
        public async Task<IActionResult> UploadProfilePic(UploadProfilePic model)
        {
            try
            {
                var resp = await _accountService.UploadProfilePic(model);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());

                return Ok(new ResponseHelper(Config).ErrorMessage(message: ex.ToString()));
            }
        }
        [Authorize]
        [HttpPost]
        [Route("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfile model)
        {
            try
            {
                var resp = await _accountService.UpdateProfile(model);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());

                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        [Authorize]
        [HttpPost]
        [Route("UpdateCompanyInfo")]
        public async Task<IActionResult> UpdateCompanyInfo(CompanyInfoDto model)
        {
            try
            {
                var resp = await _accountService.UpdateCompanyInfo(model);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());

                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        [Authorize]
        [HttpPost]
        [Route("UpdateEmail")]
        public async Task<IActionResult> UpdateEmail(UpdateProfile model)
        {
            try
            {
                var resp = await _accountService.UpdateEmail(model);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());

                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        [Authorize]
        [HttpGet]
        [Route("GetCompanyInfo")]
        public async Task<IActionResult> GetCompanyInfo()
        {
            try
            {
                var resp = await _accountService.GetCompanyInfo();
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());

                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        [Authorize]
        [HttpGet]
        [Route("GetUserRole")]
        public async Task<IActionResult> GetUserRole()
        {
            try
            {
                var resp = await _accountService.GetUserRole();
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());

                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
    }
}