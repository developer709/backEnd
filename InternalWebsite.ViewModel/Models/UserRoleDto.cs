using System;

namespace InternalWebsite.ViewModel.DTOs
{
    public partial class UserRoleDto
    {
        public long UserRoleId { get; set; }
        public string Name { get; set; }
        public int? Grade { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? EditOn { get; set; }
        public int? EditBy { get; set; }
        public bool? Active { get; set; }
    }
}