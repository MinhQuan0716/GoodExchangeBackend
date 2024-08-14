using Application.ViewModel.SubcriptionModel;
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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public Guid SubscriptionId { get; set; }
        public SubscriptionDetailViewModel subcriptionModel { get; set; }
        public int PostAmount { get; set; }
    }
}
