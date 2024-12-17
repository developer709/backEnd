using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.ViewModel.Models
{
    public class CheckTokenExpiryRequestDto
    {
        public long TokenExpiry { get; set; }
        public long RefreshExpiry { get; set; }
        public string RefreshToken { get; set; }
    }
}
