using System;

namespace InternalWebsite.ViewModel.DTOs
{
    public partial class UserInfoDto
    {
        public long UserInfoId { get; set; }
        public long? UserId { get; set; }
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string AddressId { get; set; }
        public string BaseArea { get; set; }
        public string ContactNumber { get; set; }
        public string Qualification { get; set; }
        public int? TypeId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? CreatedById { get; set; }
        public long? ModifiedById { get; set; }
    }
}