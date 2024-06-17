using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class WishList:BaseEntity
    {
        public Guid PostId { get; set; }
        public Post Post { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
