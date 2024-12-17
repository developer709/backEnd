using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Core.Entities
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }
        public string DeviceToken  { get; set; }
        public string UserId { get; set; }
        public string AllowTopics { get; set; }
        public string AllowSubTopics { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public Guid? EditBy { get; set; }
        public DateTime? EditOn { get; set; }

    }
}
