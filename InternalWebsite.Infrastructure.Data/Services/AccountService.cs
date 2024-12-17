using Dapper;
using InternalWebsite.Application.Utils;
using InternalWebsite.Application.ViewModels;
using InternalWebsite.Core.Entities;
using InternalWebsite.Infrastructure.Data.Context;
using InternalWebsite.Infrastructure.Data.Repositories;
using InternalWebsite.Infrastructure.Data.Repositories.Interfaces;
using InternalWebsite.Infrastructure.Data.Services.Interfaces;
using InternalWebsite.ViewModel.DTOs;
using InternalWebsite.ViewModel.Enum;
using InternalWebsite.ViewModel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace InternalWebsite.Infrastructure.Data.Services
{
    public class AccountService : GenericRepository<tblUser, UserInfoDto>, IAccountService
    {

        private readonly IConfiguration _configuration;
        private readonly JwtBearerTokenSettings _jwtBearerTokenSettings;
        private readonly IEmailSenderService _emailSenderService;
        private readonly UserManager<tblUser> _userManager;
        private readonly SignInManager<tblUser> _signInManager;
        private readonly IExceptionLogService _exceptionService;
        private readonly SmsInfo _smsInfo;
        private readonly ResponseHelper _responseHelper;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(ClCongDbContext context, IConfiguration configuration,
          IOptions<JwtBearerTokenSettings> jwtTokenOptions,
            IHttpContextAccessor httpContextAccessor,
            UserManager<tblUser> userManager,
            IEmailSenderService emailSenderService,
            IExceptionLogService exceptionService,
              ResponseHelper responseHelper,
              SmsInfo smsInfo,
            SignInManager<tblUser> signInManager, ClCongPrincipal clCongPrincipal
                ) : base(context,
                 httpContextAccessor, clCongPrincipal
          )
        {
            _configuration = configuration;
            _jwtBearerTokenSettings = jwtTokenOptions.Value;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSenderService = emailSenderService;
            _exceptionService = exceptionService;
            _smsInfo = smsInfo;
            _responseHelper = responseHelper;
        }

        private IDbConnection CreateConnection()
        {
            var conn = SqlConnection();
            conn.Open();
            return conn;
        }

        private SqlConnection SqlConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<AppResponse> GetUserList(string Status)
        {
            try
            {
                int user = 0;
                if (Status == "null")
                {
                    user = _context.ApplicationUsers.Count();
                }
                else
                {
                    if (Status == "true")
                        user = _context.ApplicationUsers.Where(a => a.IsActive == true).Count();
                    else
                        user = _context.ApplicationUsers.Where(a => a.IsActive == false).Count();
                }
                return new ResponseHelper(_configuration).SuccessMessage(data: user);
            }
            catch (Exception e)
            {
                return new ResponseHelper(_configuration).ErrorMessage(e);
            }
        }

        public async Task<AppResponse> SignIn(Application.ViewModels.LoginDto model)
        {
            try
            {
                //var user = await _userManager.FindByNameAsync(EncryptionHelper.DecryptData(model.Email));
                var user = await _userManager.FindByNameAsync(model.Email);
                var checkAdmin = model.Origin?.ToLower().Contains("admin.balke") ?? false;
                if (!checkAdmin)
                {
                    if (model.UserType == "google" && user == null)
                    {
                        var loginModel = new RegisterDto()
                        {
                            Email = model.Email,
                            Password = "@123ABCD!z",
                            UserType = model.UserType,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            PhoneNumber = "0000000000",
                            Picture = model.Picture,
                        };
                        return await RegisterUser(loginModel);
                    }
                    if (user == null)
                    {
                        return _responseHelper.ErrorMessage(message: "Email is incorrect.");
                        //return _responseHelper.ErrorMessage(message: "Email does not exist in the system.");
                    }
                    if (user.UserType == "google" && model.UserType == null)
                    {
                        return _responseHelper.ErrorMessage(message: "Email is associated to Google, please continue with Google");
                    }
                    if (model.UserType == "google" && string.IsNullOrEmpty(user.UserType))
                    {
                        return _responseHelper.ErrorMessage(message: "Please login using your email");
                    }
                    model.Password = !string.IsNullOrEmpty(user.UserType) ? user.UserType.ToLower() == "google" ? "@123ABCD!z" : model.Password : model.Password;
                }
                var roles = await _userManager.GetRolesAsync(user);

                var result = await _signInManager.PasswordSignInAsync(user.Email, model.Password, false, false);
                if (!result.Succeeded)
                {
                    return _responseHelper.ErrorMessage(message: "Password is incorrect.");
                    //return _responseHelper.ErrorMessage(message: "Please enter valid password.");
                }
                user.LastLogin = DateTime.UtcNow;
                _context.SaveChanges();
                var dto = new AuthenticationResultDto
                {
                    Id = user.Id,
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Picture = user.Picture,
                    UserType = user.UserType,
                    PhoneNumber = user.PhoneNumber,
                    StreetAddress1 = user.StreetAddress1,
                    StreetAddress2 = user.StreetAddress2,
                    City = user.City,
                    State = user.State,
                    ZipCode = user.ZipCode,
                    Country = user.Country,
                    IsEmailVerified = user.EmailConfirmed,
                    IsPhoneNumberVerified = user.PhoneNumberConfirmed,
                    Role = roles == null || roles.Count == 0 ? "User" : roles[0],
                };
                //if (!user.EmailConfirmed)
                //{
                //    return _responseHelper.ErrorMessage(message: "Need To Verify Email First!");
                //}
                dto.AuthToken = GenerateToken(dto);

                //if (!result.Succeeded)
                //{
                //    return _responseHelper.ErrorMessage(message: "The Email or Password is incorrect");
                //}

                return _responseHelper.SuccessMessage(message: "Login successfully!", data: dto);
            }
            catch (Exception e)
            {
                return _responseHelper.ErrorMessage(e);
            }
        }
        public async Task<AppResponse> UploadProfile(IFormFile file, string fileType)
        {
            try
            {
                //var userId = GetUserId();
                //if (userId == "")
                //    return new ResponseHelper(_configuration).ErrorMessage(message: "Sorry user is not login.");

                string ImagesDirectory = "Resources/" + fileType;
                var basePath = Directory.GetCurrentDirectory() + "/" + ImagesDirectory;
                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);
                if (file.Length > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fileExtension = Path.GetExtension(fileName).ToLower();
                    string uniqueFileName = guid.ToString() + fileExtension;

                    var fullPath = Path.Combine(basePath, uniqueFileName);
                    var path = ImagesDirectory + "/" + uniqueFileName;
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    //var userData = _context.ApplicationUsers.Where(a => a.Id.ToString() == userId).FirstOrDefault();
                    //_context.SaveChanges();
                    return new ResponseHelper(_configuration).SuccessMessage(data: path);
                }
                return new ResponseHelper(_configuration).SuccessMessage();
            }
            catch (Exception e)
            {
                return new ResponseHelper(_configuration).ErrorMessage(e);
            }
        }

        private string GenerateToken(AuthenticationResultDto identityUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtBearerTokenSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Subject = new ClaimsIdentity(new Claim[]
                //   {
                //    new Claim("Id", identityUser.Id.ToString()),
                //    new Claim("UserId", identityUser.UserId),
                //    new Claim("Email", identityUser.Email)
                //   }),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _jwtBearerTokenSettings.Audience,
                Issuer = _jwtBearerTokenSettings.Issuer,
                Expires = DateTime.UtcNow.AddDays(2),
                NotBefore = DateTime.UtcNow,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Id", identityUser.Id.ToString()),
                    new Claim("UserId", identityUser.UserId.ToString()),
                    new Claim("Email", identityUser.Email)
                }),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }



        public async Task<AppResponse> RegisterUser(RegisterDto model)
        {

            var emailExists = _context.ApplicationUsers.Where(a => a.Email.ToLower() == model.Email.ToLower()).FirstOrDefault();
            if (emailExists != null && emailExists.UserType.ToLower() == "google")
            {
                var loginModel = new Application.ViewModels.LoginDto()
                {
                    Email = model.Email,
                    Password = "@123ABCD!z",
                    UserType = model.UserType,
                };
                return await SignIn(loginModel);
            }
            if (emailExists != null)
            {
                return _responseHelper.ErrorMessage(message: "Email already exists. Please enter a valid email address");
            }
            if (string.IsNullOrEmpty(model.UserType) || !model.UserType.Equals("google", StringComparison.OrdinalIgnoreCase))
            {
                model.UserType = "";
                var phoneNumberExists = _context.ApplicationUsers.Where(a => a.PhoneNumber.ToLower() == model.PhoneNumber.ToLower()).FirstOrDefault();
                if (phoneNumberExists != null)
                {
                    return _responseHelper.ErrorMessage(message: "Phone number already exists. Please enter a valid phone number");
                }
            }
            if (!string.IsNullOrEmpty(model.UserType) && model.UserType.Equals("google", StringComparison.OrdinalIgnoreCase))
            {
                model.Password = "@123ABCD!z";
            }
            var newUser = new tblUser
            {
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.UserType.ToLower() == "google" ? "0000000000" : model.PhoneNumber,
                UserType = model.UserType,
                EmailConfirmed = model.UserType.ToLower() == "google" ? true : false,
                Picture = model.Picture,
            };
            await using var con = _context;
            var trans = con.Database.BeginTransaction();

            try
            {
                var result = await _userManager.CreateAsync(newUser, model.Password);
                if (!result.Succeeded)
                {
                    var errorMessages = result.Errors.Select(e => e.Description).ToList();
                    return _responseHelper.ErrorMessage(message: string.Join("; ", errorMessages));
                }
                if (!string.IsNullOrEmpty(model.RefranceId))
                {
                    if (Guid.TryParse(model.RefranceId, out Guid RefranceId))
                    {
                        var newRefral = new Refral
                        {
                            RefranceId = RefranceId,
                            UserId = newUser.Id,
                            IsActive = true,
                            CreatedOn = DateTime.Now,
                            CreatedBy = newUser.Id,
                        };
                        await _context.Refrals.AddAsync(newRefral);
                        await _context.SaveChangesAsync();

                        var transactionObj = new Core.Entities.Transaction
                        {
                            Amount = 25,
                            Status = ViewModel.Enum.TransactionStatus.Captured,
                            PaymentType = "Earned",
                            TransactionType = TransactionType.Earned,
                            TransactionDate = DateTime.Now,
                            CreatedBy = newUser.Id,
                            CreatedOn = DateTime.Now,
                            Currency = "USD",
                            IsActive = true,
                            SourceType = "Refrance",
                            SourceId = newUser.Id.ToString(),
                            ReferenceTransaction = GenerateDynamicId(),
                            ReferenceTraceId = newUser.Id.ToString(),
                            Description = "You earned 25 dollar from refrance signup"
                        };

                        Wallet walletObj;
                        if (_context.Wallets.Any(a => a.UserId == RefranceId))
                        {
                            walletObj = _context.Wallets.First(a => a.UserId == RefranceId);
                            walletObj.Balance += transactionObj.Amount;
                            walletObj.EditOn = DateTime.Now;
                            walletObj.EditBy = newUser.Id;

                            _context.SaveChanges();
                            transactionObj.WalletId = walletObj.Id;
                            _context.Transactions.Add(transactionObj);
                            _context.SaveChanges();
                        }
                    }
                }
                // adding Roles against newUser.
                //var userRoles = new List<tblUserRole>();
                //foreach (var roleStr in model.Roles)
                //{
                //    var roleObj = new tblUserRole();
                //    roleObj.UserId = newUser.Id;
                //    roleObj.CreatedOn = DateTime.Now;
                //    roleObj.CreatedBy = Guid.Empty;
                //    if (Guid.TryParse(roleStr, out Guid roleId))
                //        roleObj.RoleId = roleId;
                //    userRoles.Add(roleObj);
                //}
                //_context.UserRole.AddRange(userRoles);
                //_context.SaveChanges();

                await EmailConfirmation(model.Email,model.Origin);
                //if (model.UserType.ToLower() == "google")
                //{
                //    var loginModel = new Application.ViewModels.LoginDto()
                //    {
                //        Email = model.Email,
                //        Password = "@123ABCD!z",
                //        UserType = model.UserType,
                //    };
                //    return await SignIn(loginModel);
                //}
                trans.Commit();
                //if (isSend)
                //    return (new ResponseHelper(_configuration).SuccessMessage(
                //        message: "Thank you for registering! Please check your email and click on the confirmation link to verify your account",
                //        data: newUser));
                var loginModel = new Application.ViewModels.LoginDto()
                {
                    Email = model.Email,
                    Password = model.UserType == "google" ? "@123ABCD!z" : model.Password,
                    UserType = model.UserType,
                };
                return await SignIn(loginModel);
                //await EmailConfirmation(model.Email);
                //return _responseHelper.SuccessMessage(message: "User Successfully Registered!");
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return _responseHelper.ErrorMessage(ex.Message);
            }
        }
        public string GenerateDynamicId()
        {
            // Prefix
            string prefix = "txn_";

            // Timestamp part (ticks for uniqueness)
            long timestamp = DateTime.Now.Ticks;

            // Random number part
            Random random = new Random();
            int randomSuffix = random.Next(1000, 9999); // Generate a 4-digit random number

            // Combine to form the ID
            string dynamicId = $"{prefix}{timestamp}_{randomSuffix}";

            return dynamicId;
        }


        public async Task<AppResponse> EmailConfirmation(string email, string origin)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return (new ResponseHelper(_configuration).ErrorMessage(message: "user not found"));
                }
                if (user.EmailConfirmed)
                {
                    return (new ResponseHelper(_configuration).SuccessMessage(message: "Your account already confirmed!"));
                }
                var emailDto = new EmailSendDto() { Email = user.Email, FirstName = user.FirstName, LastName = user.LastName, UserName = user.UserName, UserId = user.Id, Origin = origin };
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var isSend = await EmailSend(emailDto, "emailconfirm", "Confirm Email", "home?code=", EmailAction.ConfirmEmail, token);
                if (isSend)
                    return (new ResponseHelper(_configuration).SuccessMessage(
                        message: "Thank you for registering! Please check your email and click on the confirmation link to verify your account",
                        data: user));
                else
                    return (new ResponseHelper(_configuration).ErrorMessage());
            }
            catch (Exception ex)
            {
                return (new ResponseHelper(_configuration).ErrorMessage(ex.Message));

            }
        }
        public async Task<AppResponse> PhoneNumberSend()
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Sorry user is not login.");

                var user = await _userManager.FindByIdAsync(userId);

                if (user.PhoneNumberConfirmed)
                {
                    return (new ResponseHelper(_configuration).ErrorMessage(message: "Your phone number already confirmed!"));

                }
                var sendPassword = GenerateRandomCode();

                var isSendSms = _smsInfo.VerifyPinAsync(user.PhoneNumber, sendPassword);
                var dataObj = new
                {
                    user.Email
                };
                if (isSendSms)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    AppUserVerification verfication = new AppUserVerification();
                    verfication.ApplicationUserId = user.Id;
                    verfication.VerificationCode = sendPassword;
                    verfication.VerificationToken = token;
                    verfication.Email = user.Email;
                    verfication.IsValid = true;
                    verfication.Type = EmailAction.ConfirmPhoneNumber;
                    verfication.ExpiryDate = DateTime.Now.AddDays(7);
                    verfication.CreatedDate = DateTime.Now;
                    verfication.CreatedById = 0;
                    _context.AppUserVerifications.Add(verfication);
                    _context.SaveChanges();

                }
                //trans.Commit();
                return _responseHelper.SuccessMessage(
                   message: !isSendSms
                       ? "Confirmation Phone Number wasn't Sent! Try once more!"
                       : "SMS send successfully!", data: dataObj);
            }
            catch (Exception ex)
            {
                return (new ResponseHelper(_configuration).ErrorMessage(ex.Message));

            }
        }
        public async Task<AppResponse> PhoneNumberConfirmation(string code)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Sorry user is not login.");

                var model = await _userManager.FindByIdAsync(userId);

                var objUserLocation = _context.AppUserVerifications.Where(a => a.Email == model.Email && a.Type == EmailAction.ConfirmPhoneNumber).FirstOrDefault();
                if (objUserLocation == null)
                {
                    return (new ResponseHelper(_configuration).ErrorMessage(message: "user is not valid."));
                }
                var latestVerification = _context.AppUserVerifications
                                      .Where(a => a.Email == model.Email && a.Type == EmailAction.ConfirmPhoneNumber)
                                      .OrderByDescending(a => a.CreatedDate)
                                      .FirstOrDefault();

                if (latestVerification == null)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Email is not valid.");
                }

                // Check if the OTP matches
                if (latestVerification.VerificationCode != code)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Invalid code.");
                }

                // Check if the OTP has Valid
                if (latestVerification.IsValid != true)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Confirm phone number code is not valid.");
                }
                latestVerification.IsValid = false;

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return (new ResponseHelper(_configuration).ErrorMessage(message: "user is not valid."));
                }

                if (latestVerification.VerificationCode == code)
                {
                    user.PhoneNumberConfirmed = true;
                    _context.SaveChanges();
                    return (new ResponseHelper(_configuration).SuccessMessage("Thanks for confirm your number"));
                }
                else
                {
                    return (new ResponseHelper(_configuration).ErrorMessage(message: "Invalid Code"));
                }
            }
            catch (Exception e)
            {
                return new ResponseHelper(_configuration).ErrorMessage(e);
            }
        }

        public async Task<AppResponse> CheckUsersDetails(UserDetail userDetail)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userDetail.Email);

                if (user == null)
                {
                    return _responseHelper.ErrorMessage(
                        message: "The Email is incorrect");
                }
                var roles = await _userManager.GetRolesAsync(user);


                var result = await _signInManager.PasswordSignInAsync(user.Email, userDetail.Password, false, false);

                var dto = new AuthenticationResultDto
                {
                    Id = user.Id,
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsEmailVerified = !result.IsNotAllowed,
                    Role = roles == null || roles.Count == 0 ? "Employee" : roles[0],
                };
                dto.AuthToken = GenerateToken(dto);

                //var userId1 = GetUserId();
                //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

                //var userId = GetUserId();

                //var appUserDto = new ApplicationUserSessionDto();
                //try
                //{
                //    using (var connection = CreateConnection())
                //    {
                //        var sqlQuery = "SP_Get_ApplicationUserSessions";
                //        var sqlParam = new
                //        {
                //            userId = user.Id
                //        };
                //        appUserDto = connection.Query<ApplicationUserSessionDto>(sqlQuery, sqlParam,
                //                commandType: CommandType.StoredProcedure, buffered: true, commandTimeout: 3600).FirstOrDefault();
                //    }
                //}
                //catch (Exception ex)
                //{
                //    throw;
                //}

                //if (appUserDto == null)
                //{
                //    var response = await CheckUserVerification(userDetail.VerifyCode, user.Id, dto.AuthToken, user.Email, user.UserName, userDetail.IpAddress, userDetail.BrowserName);
                //    return response;
                //}
                //else
                //{
                //    if (appUserDto.Ip == userDetail.IpAddress)
                //    {
                //        //if (appUserDto.StatusId == 1)
                //        //    return _responseHelper.ErrorMessage(message: "You have login another Browser. Please signout first");
                //        //else
                //        ApplicationUserSessionsInsertion(user.Id.ToString(), dto.AuthToken, 1, userDetail.IpAddress, userDetail.BrowserName);
                //    }
                //    else
                //    {
                //        var response = await CheckUserVerification(userDetail.VerifyCode, user.Id, dto.AuthToken, user.Email, user.UserName, userDetail.IpAddress, userDetail.BrowserName);
                //        return response;
                //    }

                //}
                return _responseHelper.SuccessMessage(data: dto);
            }
            catch (Exception e)
            {
                return _responseHelper.ErrorMessage(e);
            }
        }
        public async Task<AppResponse> GetProfileInfoById(string id)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Sorry user is not login.");
                var user = _context.ApplicationUsers.Where(a => a.FirstName.ToLower() == id.ToLower()).FirstOrDefault();
                if (user == null)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Sorry this user not exist. Please enter correct data");
                }
                using (var connection = CreateConnection())
                {
                    var sqlQuery = @$"getProfileInfo";
                    var paramObj = new
                    {
                        Id = user.Id
                    };
                    var data = await connection.QueryAsync<AuthenticationResultDto>(sqlQuery, paramObj, commandType: CommandType.StoredProcedure, commandTimeout: 3600);

                    return new ResponseHelper(_configuration).SuccessMessage(data: data);
                }
            }
            catch (Exception e)
            {
                return new ResponseHelper(_configuration).ErrorMessage(e);
            }
        }
        public async Task<AppResponse> LoginUserByCode(LoginbyCodeDto model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrWhiteSpace(model.Email))
                    return (_responseHelper.ErrorMessage(
                        message: "The Email field is required."));
                if (string.IsNullOrEmpty(model.Password) || string.IsNullOrWhiteSpace(model.Password))
                    return (_responseHelper.ErrorMessage(
                        message: "The Password field is required."));

                var user = await _userManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                    if (!result.Succeeded)
                    {
                        return (_responseHelper.ErrorMessage(message: "Invalid login or password"));
                    }
                    else
                    {
                        var getData = GetUserDetailByUserId(user.Id.ToString());
                        //var getData = getDataResult.Data;
                        if (Convert.ToDateTime(getData.LockoutEnd) > DateTime.Now)
                        {
                            if (getData.Code == model.Code)
                            {
                                return (_responseHelper.SuccessMessage("Verify User"));
                            }
                            else
                            {
                                return (_responseHelper.ErrorMessage(message: "Invalid Code"));
                            }
                        }
                        else
                        {
                            return (_responseHelper.ErrorMessage(
                                message: "your temporary password expired"));
                        }
                    }
                }

                return (_responseHelper.ErrorMessage(
                    message: "The user name or password is incorrect"));
            }
            catch (Exception e)
            {
                return _responseHelper.ErrorMessage(_exceptionService.ExceptionLog(e));
            }
        }
        private AppUserInfoDto GetUserDetailByUserId(string id)
        {
            var appUserDto = new AppUserInfoDto();
            try
            {
                using (var connection = CreateConnection())
                {
                    var sqlQuery = "GetUserDetailById";
                    var sqlParam = new
                    {
                        id = id
                    };
                    appUserDto = connection.Query<AppUserInfoDto>(sqlQuery, sqlParam,
                            commandType: CommandType.StoredProcedure, buffered: true, commandTimeout: 3600).Distinct()
                        .FirstOrDefault();

                    return appUserDto;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<AppResponse> ChangeEmail(ResendEmaillDto model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.NewEmail))
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user == null)
                    {
                        return new ResponseHelper(_configuration).SuccessMessage(
                            message: "invalid user detail");
                    }

                    var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
                    var result = await _userManager.ChangeEmailAsync(user, model.NewEmail, token);
                    var url =
                        $"{_httpContextAccessor?.HttpContext?.Request.Scheme}://{_httpContextAccessor?.HttpContext?.Request.Host}";
                    var encodedToken = HttpUtility.UrlEncode(token);
                    var emailDto = new EmailSendDto() { Email = user.Email, FirstName = user.FirstName, LastName = user.LastName, UserName = user.UserName, UserId = user.Id };
                    var isSend = await EmailSend(emailDto, "singUp", "Email Confirmation", "authentication/emailconfirmation?code=", EmailAction.ChangeEmail, token);

                    var dataObj = new
                    {
                        user.Email
                    };
                    return new ResponseHelper(_configuration).SuccessMessage(
                        message: !isSend
                            ? "Confirmation Email wasn't Sent! Try once more!"
                            : "Email Send at " + model.NewEmail + "!", data: dataObj);
                }
                else
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user == null)
                    {
                        return new ResponseHelper(_configuration).SuccessMessage(
                            message: "invalid user detail");
                    }
                    var emailDto = new EmailSendDto() { Email = user.Email, FirstName = user.FirstName, LastName = user.LastName, UserName = user.UserName, UserId = user.Id };
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var isSend = await EmailSend(emailDto, "singUp", "Email Confirmation", "authentication/emailconfirmation?code=", EmailAction.ChangeEmail, token);

                    var dataObj = new
                    {
                        user.Email
                    };

                    return new ResponseHelper(_configuration).SuccessMessage(
                        message: !isSend
                            ? "Confirmation Email wasn't Sent! Try once more!"
                            : "Email Send at " + user.Email + "!", data: dataObj);
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<AppResponse> ChangePassword(ChangePassword model)
        {
            try
            {

                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Sorry user is not login.");
                if (!string.IsNullOrEmpty(model.Id))
                    userId = model.Id;

                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return new ResponseHelper(_configuration).SuccessMessage(
                        message: "invalid user detail");
                }

                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return _responseHelper.SuccessMessage();
                }
                else
                {
                    return _responseHelper.ErrorMessage("Password is incorrect");
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }


        public async Task<AppResponse> ConfirmEmail(ConfirmEmailDto model)
        {
            try
            {
                var objUserLocation = _context.AppUserVerifications.Where(a => a.Email == model.Email && a.Type == EmailAction.ConfirmEmail).FirstOrDefault();
                if (objUserLocation == null)
                {
                    return (new ResponseHelper(_configuration).ErrorMessage(message: "email is not valid."));
                }
                var latestVerification = _context.AppUserVerifications
                                      .Where(a => a.Email == model.Email && a.Type == EmailAction.ConfirmEmail)
                                      .OrderByDescending(a => a.CreatedDate)
                                      .FirstOrDefault();

                if (latestVerification == null)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Email is not valid.");
                }

                // Check if the OTP matches
                if (latestVerification.VerificationToken != model.Code)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Invalid reset link.");
                }

                // Check if the OTP has expired
                if (latestVerification.ExpiryDate < DateTime.Now)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Confirm Email link has expired.");
                }
                // Check if the OTP has Valid
                if (latestVerification.IsValid != true)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Confirm Email link is not valid.");
                }
                latestVerification.IsValid = false;

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return (new ResponseHelper(_configuration).ErrorMessage(message: "user is not valid."));
                }

                //Compare code
                var decodedToken = HttpUtility.UrlDecode(model.Code);
                var decode2 = Uri.UnescapeDataString(model.Code);

                if (latestVerification.VerificationToken == model.Code)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    //var result = await _userManager.ConfirmEmailAsync(user, decode2);
                    user.EmailConfirmed = true;
                    _context.SaveChanges();
                    return (new ResponseHelper(_configuration).SuccessMessage("Thanks for confirm your email"));
                }
                else
                {
                    return (new ResponseHelper(_configuration).ErrorMessage(message: "Invalid Code"));
                }

                //var result = await _userManager.ConfirmEmailAsync(user, decode2);
                //return (result.Succeeded
                //    ? new ResponseHelper(_configuration).SuccessMessage()
                //    : new ResponseHelper(_configuration).ErrorMessage());
            }
            catch (Exception e)
            {
                return new ResponseHelper(_configuration).ErrorMessage(e);
            }
        }

        public async Task<AppResponse> ForgotPassword(PasswordDto model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return (new ResponseHelper(_configuration).ErrorMessage(message: "user not found"));
                }

                if (user.UserType == "google")
                {
                    return _responseHelper.ErrorMessage(message: "Invalid Login attempt.");
                }
                var emailDto = new EmailSendDto() { Email = user.Email, FirstName = user.FirstName, LastName = user.LastName, UserName = user.UserName, UserId = user.Id, Origin = model.Origin };
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var isSend = await EmailSend(emailDto, "reset", "Reset Password", "auth/reset?code=", EmailAction.ResetPassword, token);

                if (isSend)
                    return (new ResponseHelper(_configuration).SuccessMessage(
                        message: "you received a notification shortly",
                        data: user));
                else
                    return (new ResponseHelper(_configuration).ErrorMessage());
            }
            catch (Exception e)
            {
                return new ResponseHelper(_configuration).ErrorMessage(e);
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
        public async Task<AppResponse> UploadProfilePic(UploadProfilePic model)
        {
            try
            {
                var userData = await _userManager.FindByEmailAsync(model.Email);
                if (userData == null)
                {
                    return (new ResponseHelper(_configuration).ErrorMessage(message: "user not found"));
                }
                userData.Picture = model.Picture;
                _context.SaveChanges();
                return (new ResponseHelper(_configuration).SuccessMessage(message: "Save successfully!"));
            }
            catch (Exception e)
            {
                return new ResponseHelper(_configuration).ErrorMessage(e);
            }
        }
        public async Task<AppResponse> GetUserDetailById(long id = 0)
        {
            var appUserDto = new AppUserInfoDto();
            try
            {
                using (var connection = CreateConnection())
                {
                    var sqlQuery = "GetUserDetailById";
                    var sqlParam = new
                    {
                        id = id
                    };
                    appUserDto = connection.Query<AppUserInfoDto>(sqlQuery, sqlParam,
                            commandType: CommandType.StoredProcedure, buffered: true, commandTimeout: 3600).Distinct()
                        .FirstOrDefault();

                    return new ResponseHelper(_configuration).SuccessMessage(data: appUserDto);
                }
            }
            catch (Exception e)
            {
                _exceptionService.ExceptionLog(e);
                return _responseHelper.ErrorMessage(e);
            }
        }
        public AppResponse AddPasswordLog(string id, string password)
        {
            var appUserDto = new AppUserInfoDto();
            try
            {
                using (var connection = CreateConnection())
                {
                    var sqlQuery = "AddPasswordLog";
                    var sqlParam = new
                    {
                        UserId = id,
                        Password = password,
                    };
                    connection.Query<string>(sqlQuery, sqlParam, commandType: CommandType.StoredProcedure,
                        buffered: true, commandTimeout: 3600).Distinct().FirstOrDefault();

                    return new ResponseHelper(_configuration).SuccessMessage();
                }
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(
                            message: "invalid user detail");
            }
        }
        public bool ApplicationUserSessionsInsertion(string userId, string token, int LoginStatus, string IpAddress, string BrowserName)
        {
            try
            {
                //var applicationUserSecctionObj = new ApplicationUserSession()
                //{
                //    SessionToken = token,
                //    Ip = IpAddress,
                //    OS = "window",
                //    Browser = BrowserName,
                //    Device = "Laptop",
                //    LocalIp = IpAddress,
                //    UserAddress = "Karachi",
                //    UserCity = "Karachi",
                //    UserState = "Sindh",
                //    UserZip = "12311",
                //    UserCountry = "Pakistan",
                //    Latitude = 24.9056,
                //    Longitude = 67.0822,
                //    StatusId = LoginStatus,
                //    ApplicationUserId = Convert.ToInt32(userId),
                //    CreatedById = Convert.ToInt32(userId),
                //    CreatedDate = DateTime.Now,
                //    IsActive = true,
                //};

                //_applicationUserSessionRepository.Add(applicationUserSecctionObj);
                //_applicationUserSessionRepository.Save();

                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }
        public async Task<AppResponse> ResetPassword(ResetPasswordlDto model)
        {
            try
            {
                //var userData = await _userManager.FindByNameAsync(model.Username);
                var objUserLocation = _context.AppUserVerifications.Where(a => a.Email == model.Email && a.Type == EmailAction.ResetPassword).FirstOrDefault();
                if (objUserLocation == null)
                {
                    return (new ResponseHelper(_configuration).ErrorMessage(message: "email is not valid."));
                }
                var latestVerification = _context.AppUserVerifications
                                      .Where(a => a.Email == model.Email && a.Type == EmailAction.ResetPassword)
                                      .OrderByDescending(a => a.CreatedDate)
                                      .FirstOrDefault();

                if (latestVerification == null)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Email is not valid.");
                }

                // Check if the OTP matches
                if (latestVerification.VerificationToken != model.Code)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Invalid reset link.");
                }

                // Check if the OTP has expired
                if (latestVerification.ExpiryDate < DateTime.Now)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Reset link has expired.");
                }
                // Check if the OTP has Valid
                if (latestVerification.IsValid != true)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Reset link is not valid.");
                }
                latestVerification.IsValid = false;

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return (new ResponseHelper(_configuration).ErrorMessage(message: "user is not valid."));
                }

                //Compare code
                var decodedToken = HttpUtility.UrlDecode(model.Code);
                var decode2 = Uri.UnescapeDataString(model.Code);

                if (latestVerification.VerificationToken == model.Code)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
                    if (result.Succeeded)
                    {
                        AddPasswordLog(user.Id.ToString(), model.Password);
                        _context.SaveChanges();
                        return (new ResponseHelper(_configuration).SuccessMessage());
                    }
                    else
                    {
                        var errors = result.Errors.Select(e => e.Description).ToArray();
                        string errorMessage = string.Join("; ", errors);
                        return (new ResponseHelper(_configuration).ErrorMessage(message: errorMessage));
                    }
                }
                else
                {
                    return (new ResponseHelper(_configuration).ErrorMessage(message: "Invalid Code"));
                }
            }
            catch (Exception e)
            {
                return new ResponseHelper(_configuration).ErrorMessage(e);
            }
        }
        public async Task<AppResponse> GetUserDetails()
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Sorry user is not login.");

                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(
                    message: "Incorrect User Reference. Are you logged in?");
                }
                //if (orgId == null)
                //{
                //    return new ResponseHelper(_configuration).ErrorMessage(
                //        message: "Incorrect User Reference. Are you logged in?");
                //}
                //var userInfo = _userInfoRepository.FirstOrDefault(f => f.UserId == user.Id);
                var roles = await _userManager.GetRolesAsync(user);

                //var userRole = 
                var dto = new AuthenticationResultDto
                {
                    Id = user.Id,
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsEmailVerified = user.EmailConfirmed,
                    //Role = roles == null || roles.Count == 0 ? "Employee" : roles[0],
                };

                return new ResponseHelper(_configuration).SuccessMessage(data: dto);
            }
            catch (Exception e)
            {
                return new ResponseHelper(_configuration).ErrorMessage(e);
            }
        }
        public async Task<AppResponse> GetUserDetailById()
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Sorry user is not login.");

                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(
                        message: "Incorrect User Reference. Are you logged in?");
                }
                //if (orgId == null)
                //{
                //    return new ResponseHelper(_configuration).ErrorMessage(
                //        message: "Incorrect User Reference. Are you logged in?");
                //}
                //var userInfo = _userInfoRepository.FirstOrDefault(f => f.UserId == user.Id);
                var roles = await _userManager.GetRolesAsync(user);

                //var userRole = 
                var dto = new AuthenticationResultDto
                {
                    Id = user.Id,
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    IsEmailVerified = user.EmailConfirmed,
                    Role = roles == null || roles.Count == 0 ? "Employee" : roles[0],
                };
                return new ResponseHelper(_configuration).SuccessMessage(data: dto);
            }
            catch (Exception e)
            {
                return new ResponseHelper(_configuration).ErrorMessage(e);
            }
        }
        public async Task<bool> EmailSend(EmailSendDto user, string emailType, string emailTitle, string redirectUrl, EmailAction emailAction, string token)
        {
            var sendPassword = GenerateRandomCode();


            AppUserVerification verfication = new AppUserVerification();
            verfication.ApplicationUserId = user.UserId;
            verfication.VerificationCode = sendPassword;
            verfication.VerificationToken = token;
            verfication.Email = user.Email;
            verfication.IsValid = true;
            verfication.Type = emailAction;
            verfication.ExpiryDate = DateTime.Now.AddDays(7);
            verfication.CreatedDate = DateTime.Now;
            verfication.CreatedById = 0;
            _context.AppUserVerifications.Add(verfication);
            _context.SaveChanges();

            var url =
                $"{_httpContextAccessor?.HttpContext?.Request.Scheme}://{_httpContextAccessor?.HttpContext?.Request.Host}";
            var encodedToken = HttpUtility.UrlEncode(token);
            var checkAdmin = false;
            if (!string.IsNullOrEmpty(user.Origin))
            {
                checkAdmin = user.Origin.ToLower().Contains("admin.balke");
                if (emailType == "reset" && checkAdmin)
                {
                    emailType = "adminreset";
                }

            }
            var callbackUrl = checkAdmin ? _configuration["Target:Admin_Url_Prod"].ToString() : _configuration["Target:Url_Dev"].ToString();
            callbackUrl = callbackUrl + redirectUrl +
                              encodedToken +
                              "&email=" + user.Email;

            var emailSendModel = new EmailSendDto() { CallBackUrl = callbackUrl, Email = user.Email, UserName = user.UserName, EmailTitle = emailTitle, EmailType = emailType, FirstName = user.FirstName, LastName = user.LastName, OTP = sendPassword };

            bool isSend = _emailSenderService.SendSingUpGroupEmail(emailSendModel);
            return isSend;
        }
        public async Task<AppResponse> UpdateProfile(UpdateProfile model)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }

                var userData = _context.ApplicationUsers.Where(a => a.Id == userId).FirstOrDefault();
                if (userData == null)
                {
                    return (new ResponseHelper(_configuration).ErrorMessage(message: "user not found"));
                }

                if (model.SectionType == "fullname")
                {
                    userData.FirstName = model.FirstName;
                    userData.LastName = model.LastName;
                    model.SectionType = "Full Name";
                }
                else if (model.SectionType == "profilepic")
                {
                    userData.Picture = model.Picture;
                    model.SectionType = "Profile Pic";
                }
                else if (model.SectionType == "email")
                {
                    if (userData != null && userData.UserType.ToLower() == "google")
                    {
                        return (new ResponseHelper(_configuration).ErrorMessage(message: "You are register with google. You did not change your email."));
                    }
                    var newEmailExists = await _context.ApplicationUsers.Where(a => a.Email.ToLower() == model.NewEmail.ToLower()).FirstOrDefaultAsync();
                    if (newEmailExists != null)
                    {
                        return _responseHelper.ErrorMessage(message: "Email already exists. Please enter a valid email address");
                    }

                    var emailDto = new EmailSendDto() { Email = model.NewEmail, FirstName = userData.FirstName, LastName = userData.LastName, UserName = userData.UserName, UserId = userData.Id };
                    var token = await _userManager.GenerateChangeEmailTokenAsync(userData, model.NewEmail);
                    var isSend = await EmailSend(emailDto, "changeemail", "Change Email", "", EmailAction.ChangeEmail, token);
                    if (isSend)
                        return (new ResponseHelper(_configuration).SuccessMessage(
                            message: "Please check your email and put the OTP in the given boxes"));
                    else
                        return (new ResponseHelper(_configuration).ErrorMessage());
                }
                else if (model.SectionType == "contact")
                {
                    if (userData.PhoneNumber == model.PhoneNumber)
                    {
                        return (new ResponseHelper(_configuration).ErrorMessage(message: "This phone number already register. Please use another."));
                    }
                    userData.PhoneNumber = model.PhoneNumber;
                    model.SectionType = "Phone No";
                }
                else if (model.SectionType == "location")
                {
                    userData.StreetAddress1 = model.StreetAddress1;
                    userData.StreetAddress2 = model.StreetAddress2;
                    userData.City = model.City;
                    userData.State = model.State;
                    userData.ZipCode = model.ZipCode;
                    userData.Country = model.Country;
                    model.SectionType = "Location";
                }

                _context.SaveChanges();
                return (new ResponseHelper(_configuration).SuccessMessage(message: model.SectionType + " Update successfully!"));
            }
            catch (Exception e)
            {
                return new ResponseHelper(_configuration).ErrorMessage(e);
            }
        }
        public async Task<AppResponse> UpdateCompanyInfo(CompanyInfoDto model)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }

                var userData = _context.ApplicationUsers.Where(a => a.Id == userId).FirstOrDefault();
                if (userData == null)
                {
                    return (new ResponseHelper(_configuration).ErrorMessage(message: "user not found"));
                }
                var getcompanyInfo = _context.CompanyInfoes.Where(a => a.UserId == userId).FirstOrDefault();
                if (getcompanyInfo == null)
                {
                    var company = new CompanyInfo();
                    company.UserId = userId;
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
                return (new ResponseHelper(_configuration).SuccessMessage(message: "Save successfully!"));
            }
            catch (Exception e)
            {
                return new ResponseHelper(_configuration).ErrorMessage(e);
            }
        }
        public async Task<AppResponse> UpdateEmail(UpdateProfile model)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }

                var user = _context.ApplicationUsers.Where(a => a.Id == userId).FirstOrDefault();
                if (user == null)
                {
                    return (new ResponseHelper(_configuration).ErrorMessage(message: "user not found"));
                }
                var objUserLocation = _context.AppUserVerifications.Where(a => a.Email == model.NewEmail && a.Type == EmailAction.ChangeEmail).FirstOrDefault();
                if (objUserLocation == null)
                {
                    return (new ResponseHelper(_configuration).ErrorMessage(message: "email is not valid."));
                }

                var latestVerification = _context.AppUserVerifications
                                      .Where(a => a.Email == model.NewEmail && a.Type == EmailAction.ChangeEmail)
                                      .OrderByDescending(a => a.CreatedDate)
                                      .FirstOrDefault();

                if (latestVerification == null)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Email is not valid.");
                }

                // Check if the OTP matches
                if (latestVerification.VerificationCode != model.OTP)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Invalid OTP.");
                }

                // Check if the OTP has expired
                if (latestVerification.ExpiryDate < DateTime.Now)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "OTP has expired.");
                }
                // Check if the OTP has Valid
                if (latestVerification.IsValid != true)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "OTP is not valid.");
                }
                latestVerification.IsValid = false;

                //Compare code

                if (latestVerification.VerificationCode == model.OTP)
                {
                    //var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
                    var result = await _userManager.ChangeEmailAsync(user, model.NewEmail, latestVerification.VerificationToken);
                    if (result.Succeeded)
                    {
                        user.UserName = model.NewEmail;
                        await _userManager.UpdateNormalizedUserNameAsync(user);

                        _context.SaveChanges();
                        return (new ResponseHelper(_configuration).SuccessMessage("Email Update Scucessfully"));
                    }
                    else
                    {
                        var errors = result.Errors.Select(e => e.Description).ToArray();
                        string errorMessage = string.Join("; ", errors);
                        return (new ResponseHelper(_configuration).ErrorMessage(message: errorMessage));
                    }
                }
                else
                {
                    return (new ResponseHelper(_configuration).ErrorMessage(message: "Invalid Code"));
                }
            }
            catch (Exception e)
            {
                return new ResponseHelper(_configuration).ErrorMessage(e);
            }
        }

        public async Task<AppResponse> GetCompanyInfo()
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }

                var companyInfo = _context.CompanyInfoes.Where(a => a.UserId == userId).FirstOrDefault();

                return (new ResponseHelper(_configuration).SuccessMessage(data: companyInfo));

            }
            catch (Exception e)
            {
                return new ResponseHelper(_configuration).ErrorMessage(e);
            }
        }
        public async Task<AppResponse> GetUserRole()
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }
                var user = await _userManager.FindByIdAsync(userId.ToString());

                var roles = await _userManager.GetRolesAsync(user);

                return (new ResponseHelper(_configuration).SuccessMessage(data: roles));

            }
            catch (Exception e)
            {
                return new ResponseHelper(_configuration).ErrorMessage(e);
            }
        }
    }
}
