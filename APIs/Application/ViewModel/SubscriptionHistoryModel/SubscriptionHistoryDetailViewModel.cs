using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.SubscriptionHistoryModel
{
    public class SubscriptionHistoryDetailViewModel
    {
        public Guid Id { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Status { get; set; }
        public Guid SubscriptionId { get; set; }
    }
}
