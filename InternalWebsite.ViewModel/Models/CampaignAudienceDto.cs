﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.ViewModel.Models
{
    public class CampaignAudienceDto
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public string CityDetails { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public string CampaignId { get; set; }
        public int StartAge { get; set; }
        public int EndAge { get; set; }
    }
}