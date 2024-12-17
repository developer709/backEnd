using System;
using InternalWebsite.ViewModel.DTOs;

namespace InternalWebsite.ViewModel.DTOs
{
    public partial class UserLoginDto
    {
        public long UserLoginId { get; set; }
        public long? UserInfoId { get; set; }
        public UserInfoDto UserInfo { get; set; }
        public string Username { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string Pin { get; set; }
        public int? UserRoleId { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? EditOn { get; set; }
        public int? EditBy { get; set; }
        public bool? Active { get; set; }
    }
}