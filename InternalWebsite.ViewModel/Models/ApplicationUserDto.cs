using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InternalWebsite.ViewModel.DTOs
{
    public partial class ApplicationUserDto
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string NewEmail { get; set; }
        public string Code { get; set; }
        public string ProfilePic { get; set; }
        public bool IsInitialSetupDone { get; set; }
        public string Permission { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? CreatedById { get; set; }
        public long? ModifiedById { get; set; }
        public long Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string ActionType { get; set; }
    }

    public partial class RegisterDto
    {
        public string RefranceId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Picture { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; }
        public string Origin { get; set; }
        public string GRecaptchaResponse { get; set; }
        public List<string> Roles { get; set; }
    }
    public partial class UploadProfilePic
    {
        public string Email { get; set; }
        public string Picture { get; set; }
    }
    public partial class ConfirmEmailDto
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
    public partial class PasswordDto
    {
        public string GRecaptchaResponse { get; set; }
        public string Email { get; set; }
        public string Origin { get; set; }
        //public string Code { get; set; }
    }
    public partial class ResetPasswordlDto
    {
        public string Password { get; set; }
        public string GRecaptchaResponse { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
    }
    public partial class ResendEmaillDto
    {
        public string Email { get; set; }
        public string NewEmail { get; set; }
    }
    public partial class ChangePassword
    {
        public string Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
    }
    public partial class UpdateProfile
    {
        //public string Email { get; set; }
        public string NewEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Picture { get; set; }
        public string OTP { get; set; }
        public string SectionType { get; set; }
        [StringLength(100)]
        public string StreetAddress1 { get; set; }
        [StringLength(100)]
        public string StreetAddress2 { get; set; }
        [StringLength(50)]
        public string City { get; set; }
        [StringLength(50)]
        public string State { get; set; }
        [StringLength(10)]
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }
    public class EmailSendDto
    {
        public Guid? UserId { get; set; }
        public string Email { get; set; }
        public string EmailTitle { get; set; }
        public string UserName { get; set; }
        public string EmailType { get; set; }
        public string CallBackUrl { get; set; }
        public string OTP { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Origin { get; set; }
    }
    //public partial class LogoutDto
    //{
    //    public string Email { get; set; }
    //    public string NewEmail { get; set; }
    //}
}