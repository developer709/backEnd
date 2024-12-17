using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.ViewModel.Models
{
    public class PaymentRequestDto
    {
        public string Id { get; set; }
        public int Amount { get; set; }
        public bool SaveCard { get; set; }
        public string PostUrl { get; set; }
        public string RedirectUrl { get; set; }
        public string StatementDescriptor { get; set; }
    }
}
