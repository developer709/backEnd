using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.ViewModel.Models
{
    public class RefundDto
    {
        public string ChargeId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Reason { get; set; }
        public string PostUrl { get; set; }
        public virtual ICollection<DestinationDto> Destinations { get; set; }
    }
}
