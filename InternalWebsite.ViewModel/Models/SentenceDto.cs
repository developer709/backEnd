using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.ViewModel.Models
{
    public class SentenceDto
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public int Size { get; set; }
        public string Language { get; set; }
    }
}
