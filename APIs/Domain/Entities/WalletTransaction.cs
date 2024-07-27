using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class WalletTransaction:BaseEntity
    {
        public string TransactionType { get; set; }
        public float Amount { get; set; }
        public Guid WalletId { get; set; }  
        public Wallet Wallet { get; set; }  
        public Guid? SubscriptionId { get; set; }
        public Subscription Subcription { get; set; }
        public Guid? OrderId { get; set; }
        public Order Order { get; set; }
    }
}
