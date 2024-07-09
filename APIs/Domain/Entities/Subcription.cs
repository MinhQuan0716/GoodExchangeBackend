using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public  class Subcription:BaseEntity
    {
        public long Price { get; set; }
        public string Description { get; set; }
        public string SubcriptionType { get; set; }
        public Guid? WalletTransactionId { get; set; }
        public float ExpiryMonth { get; set; }
        public WalletTransaction WalletTransaction { get; set; }
        public ICollection<SubcriptionHistory> SubcriptionHistories { get; set; }
    }
}
