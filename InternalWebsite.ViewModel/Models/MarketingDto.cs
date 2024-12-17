using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.ViewModel.Models
{
    public class MarketingDto
    {
        public string Id { get; set; }
        public string BusinessName { get; set; }
        public string OwnerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Objective { get; set; }
        public string Summary { get; set; }
        public int Duration { get; set; }
        public string Type { get; set; }
        public float Budget { get; set; }
        public string Solution { get; set; }
        public string Details { get; set; }
    }
}
