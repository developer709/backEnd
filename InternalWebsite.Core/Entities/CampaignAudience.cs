using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Core.Entities
{
    public class CampaignAudience
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public string CityDetails { get; set; }
        public string Gender { get; set; }
        public int StartAge { get; set; }
        public int EndAge { get; set; }
        public string Age { get; set; }
        public Guid CampaignId { get; set; }
        public virtual Campaign Campaign { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public Guid CreatedBy { get; set; }
        public DateTime? EditOn { get; set; }
        public Guid? EditBy { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
