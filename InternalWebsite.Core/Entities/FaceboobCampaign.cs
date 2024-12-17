using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Core.Entities
{
    public class FacebookCampaign
    {
        public Guid Id { get; set; }
        public Guid CampaignId { get; set; }
        public string FbCampaignId { get; set; }
        public string AdSetId { get; set; }
        public string AdCreativeId { get; set; }
        public string AdId { get; set; }
        public string Status { get; set; }
        public string MediaType { get; set; }
        public Guid UserId { get; set; }
        public virtual tblUser User { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public Guid CreatedBy { get; set; }
        public DateTime? EditOn { get; set; }
        public Guid? EditBy { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
