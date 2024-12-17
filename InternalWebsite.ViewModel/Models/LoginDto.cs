using System;

namespace InternalWebsite.Application.ViewModels
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string UserType { get; set; }
        public string Password { get; set; }
        //public string VerifyCode { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string Picture { get; set; }
        public string Origin { get; set; }
        public string GRecaptchaResponse { get; set; }
    }
    public class LoginbyCodeDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Code { get; set; }
    }
    public class UserDetail
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string VerifyCode { get; set; }
        public string IpAddress { get; set; }
        public string BrowserName { get; set; }
    }
}
