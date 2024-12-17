using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Core.Entities
{
    public class Refral
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RefranceId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public Guid CreatedBy { get; set; }
        public DateTime? EditOn { get; set; }
        public Guid? EditBy { get; set; }
        public virtual tblUser User { get; set; }
    }
}
