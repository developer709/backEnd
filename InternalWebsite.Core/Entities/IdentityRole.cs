using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Core.Entities
{
    public class IdentityRole : IdentityRole<Guid>
    {
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? EditOn { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? EditBy { get; set; }
    }
}
