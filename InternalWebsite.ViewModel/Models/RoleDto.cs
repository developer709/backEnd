using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.ViewModel.Models
{
    public class RoleDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
