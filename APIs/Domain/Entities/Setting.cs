using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Policy:BaseEntity
    {
        public float PostPrice { get; set; }
        public int OrderCancelledAmount { get; set; }
    }
}
