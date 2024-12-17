using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.ViewModel.Models
{
    public class IntegrationDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Provider { get; set; }
        public string Status { get; set; }
        public string Data { get; set; }
        public string Info { get; set; }
        public string PageId { get; set; }
        public string SessionId { get; set; }
    }
}
