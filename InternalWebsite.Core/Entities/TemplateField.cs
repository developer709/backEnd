using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Core.Entities
{
    public class TemplateField
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TemplateFieldId { get; set; }
        public long EmailTemplateId { get; set; }
        public virtual EmailTemplate EmailTemplate { get; set; }
        public long? TargetedId { get; set; }
        [StringLength(30)]
        public string FieldName { get; set; }
        [StringLength(30)]
        public string Type { get; set; }
        public long? CreatedById { get; set; }
    }
}
