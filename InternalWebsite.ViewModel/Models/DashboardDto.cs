using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.ViewModel.Models
{
    public class DashboardDto
    {

    }

    public class SocialCampaign
    {
        public int Snapchat { get; set; }
        public int Facebook { get; set; }
        public int Tiktok { get; set; }
        public int Twitter { get; set; }
        public int Instagram { get; set; }
        public int Total { get; set; }
    }
    public class TotalCampaignBudget
    {
        public decimal TotalBudget { get; set; }
        public decimal Earning { get; set; }
    }

}
