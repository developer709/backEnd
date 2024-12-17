using System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace InternalWebsite.Core.Entities
{
    public class tblUser : IdentityUser<Guid>
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
        public DateTime LastLogin { get; set; } = DateTime.UtcNow;
        public string Country { get; set; }
        public bool? IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? EditOn { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? EditBy { get; set; }

    }
}