
using System;

namespace InternalWebsite.Application.ViewModels
{
    public class AuthenticationResultDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AuthToken { get; set; }
        public bool? IsDefault { get; set; }
        public string Role { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsPhoneNumberVerified { get; set; }
        public bool? IsPublic { get; set; } 
        public string UserName { get; set; }
        public string Picture { get; set; }
        public string UserType { get; set; }
        public string PhoneNumber { get; set; }
        public string StreetAddress1 { get; set; }
        public string StreetAddress2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }

    }

}
