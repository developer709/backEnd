using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.ViewModel.Models
{
    public class CampaignContentDto
    {
        public string Id { get; set; }
        public string BrandName { get; set; }
        public string Headline { get; set; }
        public string BrandLogo { get; set; }
        public string Media { get; set; }
        public string MediaType { get; set; }
        public bool AdFavouriting { get; set; }
        public bool AdDisclaimer { get; set; }
        public string Disclaimer { get; set; }
        public bool BtnColor { get; set; }
        public int? ThumbnailCollection { get; set; }
        public string MediaFormat { get; set; }
        public bool SearchFeed { get; set; }
        public int NumberOfVideo { get; set; }
        public string Attachment { get; set; }
        public string CTA { get; set; }
        public string CampaignId { get; set; }
        public string PageName { get; set; }
        public string AdFormat { get; set; }
        public string Caption { get; set; }
        public string Destination { get; set; }
        public string WebUrl { get; set; }
        public string MoreURL { get; set; }
        public string DisplayURL { get; set; }
        public string Description { get; set; }
        public List<CampaignContentCollectionDto> CampaignContentCollections { get; set; }
        public List<TiktokIdentityContentCollectionDto> TiktokIdentityContentCollections { get; set; }
    }
}
