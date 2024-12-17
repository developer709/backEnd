using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using InternalWebsite.ViewModel.Enum;

namespace InternalWebsite.Core.Entities
{
    public class AppUserVerification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AppUserVerificationId { get; set; }

        public EmailAction Type { get; set; }
        [StringLength(500)]
        public string VerificationToken { get; set; }
        [StringLength(10)]
        public string VerificationCode { get; set; }
        //public ApplicationUser AppUser { get; set; }
        public Guid? ApplicationUserId { get; set; }
        public string Email { get; set; }
        public bool IsValid { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? TempPasswordTimeOut { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
        public long? CreatedById { get; set; }
        //public ApplicationUser CreatedBy { get; set; }
    }
}
