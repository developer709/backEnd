using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternalWebsite.Core.Interfaces
{
    public interface IGeneralBase : IMinBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public bool? IsActive { get; set; }
        //public bool? IsDelete { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedById { get; set; }
        //[ForeignKey("CreatedById")]
        //public ApplicationUser CreatedBy { get; set; }
        public Guid? ModifiedById { get; set; }
        //[ForeignKey("ModifiedById")]
        //public ApplicationUser ModifiedBy { get; set; }
    }
}