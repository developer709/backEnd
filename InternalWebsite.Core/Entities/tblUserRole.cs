using System;
using System.ComponentModel.DataAnnotations;
namespace InternalWebsite.Core.Entities
{
    //NOTE: To Remove
    public partial class tblUserRole
    {
        [Key]
        public Guid UserRoleId { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public Guid CreatedBy { get; set; }
    }
}
