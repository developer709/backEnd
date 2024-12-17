using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Core.Entities
{
    public class TiktokIdentityContentCollection
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Image { get; set; }
        public Guid CampaignId { get; set; }
        public virtual Campaign Campaign { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public Guid CreatedBy { get; set; }
        public DateTime? EditOn { get; set; }
        public Guid? EditBy { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
