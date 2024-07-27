using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SubscriptionHistory:BaseEntity
    {
        public long Rate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Status { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid SubcriptionId { get; set; }
        public Subscription Subcription { get; set; }
    }
}
