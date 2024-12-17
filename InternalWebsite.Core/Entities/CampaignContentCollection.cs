using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Core.Entities
{
   public class CampaignContentCollection
    {
        public Guid Id { get; set; }
        public string Image { get; set; }
        public string Heading { get; set; }
        public string Description { get; set; }
        public string WebUrl { get; set; }
        public string ImageHash { get; set; }
        public string ImageName { get; set; }
        public string Caption { get; set; }
        public string Headline { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public Guid CampaignId { get; set; }
        public virtual Campaign Campaign { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public Guid CreatedBy { get; set; }
        public DateTime? EditOn { get; set; }
        public Guid? EditBy { get; set; }
        public bool? IsActive { get; set; } = true;
       
    }
}
