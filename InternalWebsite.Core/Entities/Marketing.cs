using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Core.Entities
{
    public class Marketing
    {
        public Guid Id { get; set; }
        public string BusinessName { get; set; }
        public string OwnerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Objective { get; set; }
        public int Duration { get; set; }
        public string Type { get; set; }
        public string Summary { get; set; }
        public float Budget { get; set; }
        public string Solution { get; set; }
        public string Details { get; set; }
        public Guid UserId { get; set; }
        public virtual tblUser User { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public Guid CreatedBy { get; set; }
        public DateTime? EditOn { get; set; }
        public Guid? EditBy { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
