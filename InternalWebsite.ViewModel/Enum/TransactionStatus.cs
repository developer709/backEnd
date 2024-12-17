using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.ViewModel.Enum
{
    public enum TransactionStatus
    {
        Captured,
        Declined,
        Authorized,
        Pending,
        Failed,
        Timeout,
        Match,
        Not_Processed,
        No_Match,
        Unknown,
    }
}
