using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace InternalWebsite.ViewModel.Models
{
    public class WalletDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Currency { get; set; }
        public decimal Balance { get; set; } = 0.00m;
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public Guid CreatedBy { get; set; }
        public DateTime? EditOn { get; set; }
        public Guid? EditBy { get; set; }
        // Navigation properties
        //public virtual tblUser User { get; set; }
        public virtual ICollection<TransactionDto> Transactions { get; set; }
    }
}
