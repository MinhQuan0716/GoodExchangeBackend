using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public  class Subscription:BaseEntity
    {
        public long Price { get; set; }
        public string Description { get; set; }
        public string SubcriptionType { get; set; }
        public float ExpiryMonth { get; set; }
        public ICollection<WalletTransaction> WalletTransactions { get; set; }
        public ICollection<SubscriptionHistory> SubcriptionHistories { get; set; }
    }
}
