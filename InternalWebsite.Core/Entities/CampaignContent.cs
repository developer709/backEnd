using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InternalWebsite.Core.Entities
{
    public class CampaignContent
    {
        public Guid Id { get; set; }
        public string BrandName { get; set; }
        public string Headline { get; set; }
        public string BrandLogo { get; set; }
        public string Media { get; set; }
        public string MediaType { get; set; }
        public string MediaFormat { get; set; }
        public string Attachment { get; set; }
        public string CTA { get; set; }
        public string PageName { get; set; }
        public string AdFormat { get; set; }
        public string Caption { get; set; }
        public bool AdFavouriting { get; set; }
        public bool AdDisclaimer { get; set; }
        public string Disclaimer { get; set; }
        public bool SearchFeed { get; set; }
        public int NumberOfVideo { get; set; }
        public bool BtnColor { get; set; }
        public int? ThumbnailCollection { get; set; }
        public string Destination { get; set; }
        public string WebUrl { get; set; }
        public string MoreURL { get; set; }
        public string DisplayURL { get; set; }
        public string Description { get; set; }
        public Guid CampaignId { get; set; }
        public virtual Campaign Campaign { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public Guid CreatedBy { get; set; }
        public DateTime? EditOn { get; set; }
        public Guid? EditBy { get; set; }
        public bool? IsActive { get; set; } = true;

        [NotMapped]
        public virtual List<CampaignContentCollection> CampaignContentCollections { get; set; }
        [NotMapped]
        public virtual List<TiktokIdentityContentCollection> TiktokIdentityContentCollections { get; set; }
    }
}
