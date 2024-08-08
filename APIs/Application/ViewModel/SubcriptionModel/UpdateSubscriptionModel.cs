using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.SubcriptionModel
{
    public class UpdateSubscriptionModel
    {
        public Guid Id { get; set; }
        public long Price { get; set; }
        public string SubcriptionType { get; set; }
        public float ExpiryMonth { get; set; }
    }
}
