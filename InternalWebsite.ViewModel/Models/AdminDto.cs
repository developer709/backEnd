using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.ViewModel.Models
{
    public class AdminDto
    {
        public string Id { get; set; }
    }
    public class AdminUserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public string Type { get; set; }
        public DateTime SignUpDate { get; set; }
        public DateTime LastLogin { get; set; }
    }
    public class UserCampaignSummaryDto
    {
        public string UserId { get; set; } // Or Guid if that's the type of CreatedBy
        public decimal TotalBudget { get; set; }
        public int TotalCampaigns { get; set; }
    }
    public class AdminUserWithCampaignDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public DateTime SignUpDate { get; set; }
        public DateTime LastLogin { get; set; }
        public decimal TotalBudget { get; set; }
        public int TotalCampaigns { get; set; }
    }
    public class AdminUser
    {
        //public string Email { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string Name { get; set; }
        public string VatNumber { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Picture { get; set; }
        public string UserType { get; set; }

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
    public class UserCompanyInfoDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserType { get; set; }
        public string Picture { get; set; }
        public string StreetAddress1 { get; set; }
        public string StreetAddress2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }    // Company name
        public string VatNumber { get; set; }      // VAT number
        public List<string> Roles { get; set; }
    }

}
