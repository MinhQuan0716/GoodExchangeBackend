using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.RequestModel
{
    public class CreateOrderModel
    {
        public Guid PostId { get; set; }
        public string OrderMessage { get; set; }
        public Guid AuthorId { get; set; }
    }
}
