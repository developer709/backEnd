using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.ViewModel.Models
{
    public class NotificationDto
    {
        public string Id { get; set; }
        public string DeviceToken { get; set; }
        public string UserId { get; set; }
        public string AllowTopics { get; set; }
        public string AllowSubTopics { get; set; }
    }
}
