﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.ViewModel.Models
{
    public class LookupDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Role { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
        public string LookupParentId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public Guid CreatedBy { get; set; }
        public DateTime? EditOn { get; set; }
        public Guid? EditBy { get; set; }
    }
}
