using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Core.Entities
{
    public class EmailTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EmailTemplateId { get; set; }
        [StringLength(30)]
        public string Name { get; set; }
        //[StringLength(1)]
        public string Content { get; set; }
        public bool IsHtml { get; set; }
        public bool HasTemplateAttribute { get; set; }
        public long? CreatedById { get; set; }
        public long? ModifiedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
