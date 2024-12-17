using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.ViewModel.DTOs
{
    public class PaginatorDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int Total { get; set; } = 0;
        public int Pages { get; set; } = 0;
        public string Search { get; set; } = "";
        public bool Internal { get; set; } = false;
        public int OrganizationId { get; set; }
        public int TypeId { get; set; } = 0;
    }
}
