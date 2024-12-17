using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Core.Entities
{
    public class Campaign
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string AdType { get; set; }
        public string Title { get; set; }
        public string Objective { get; set; }
        public string Status { get; set; }
        public Guid UserId { get; set; }
        public virtual tblUser User { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public Guid CreatedBy { get; set; }
        public DateTime? EditOn { get; set; }
        public Guid? EditBy { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
