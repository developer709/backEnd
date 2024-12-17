using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.ViewModel.Models
{
    public class CardTokenDto
    {
        public string CardNumber { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public int Cvc { get; set; }
        public string CardHolderName { get; set; }
        public string Country { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressCity { get; set; }
        public string AddressStreet { get; set; }
        public string AddressAvenue { get; set; }
        public string ClientIp { get; set; }
    }
}
