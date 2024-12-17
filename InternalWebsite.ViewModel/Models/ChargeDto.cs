using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.ViewModel.Models
{
    public class ChargeDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Type { get; set; }
        public string Limit { get; set; }
        public string OrderBy { get; set; }
        public string? Merchants { get; set; } = null;
        public string? TransactionId { get; set; } = null;
        public string? OrderId { get; set; } = null;
    }
}
