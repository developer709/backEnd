using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.ViewModel.Models
{
    public class CampaignBudgetDto
    {
        public string Id { get; set; }
        public string ScheduleType { get; set; }
        public int NumberOfDay { get; set; }
        public float Budget { get; set; }
        public string CampaignId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? EndTime { get; set; }
        public bool HasEndDate { get; set; }

    }
}
