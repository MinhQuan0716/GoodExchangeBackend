using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Order:BaseEntity
    {
        public string OrderMessage { get; set; }
        public int OrderStatusId { get; set; }
        public OrderStatus Status { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid PostId { get; set; }
        public Post Post { get; set; }
        public ICollection<WalletTransaction > Transactions { get; set; }    
    }
}
