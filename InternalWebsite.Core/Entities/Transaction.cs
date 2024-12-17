using InternalWebsite.ViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Transactions;

namespace InternalWebsite.Core.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentType { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public ViewModel.Enum.TransactionStatus Status { get; set; } = ViewModel.Enum.TransactionStatus.Pending;
        public string CardBrand { get; set; }
        public string CardFirstEight { get; set; }
        public string CardLastFour { get; set; }
        public string CardSecurity { get; set; }
        public string ReceiptId { get; set; }
        public bool ThreeDSecure { get; set; }
        public string ThreeDSecureId { get; set; }
        public string SourceId { get; set; }
        public string SourceType { get; set; }
        public string SourceChannel { get; set; }
        public string ReferenceAcquirer { get; set; }
        public string ReferenceGateway { get; set; }
        public string ReferenceOrder { get; set; }
        public string ReferencePayment { get; set; }
        public string ReferenceTraceId { get; set; }
        public string ReferenceTrack { get; set; }
        public string ReferenceTransaction { get; set; }
        public string VerfiyId { get; set; }
        public string VerfiyOrderId { get; set; }
        public string PayerAuthId { get; set; }
        public string CustomerId { get; set; }
        public bool LiveMode { get; set; }
        public bool IsActive { get; set; }
        public string TimeZone { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public Guid CreatedBy { get; set; }
        public DateTime? EditOn { get; set; }
        public Guid? EditBy { get; set; }
        // Navigation property
        [JsonIgnore]
        public virtual Wallet Wallet { get; set; }
    }
}
