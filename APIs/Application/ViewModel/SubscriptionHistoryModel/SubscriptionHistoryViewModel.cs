using Application.ViewModel.SubcriptionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.SubscriptionHistoryModel
{
    public class SubscriptionHistoryViewModel
    {
        public string UsertName { get; set; }
        public string Email { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public SubscriptionDetailViewModel subcriptionModel { get; set; }
    }
}
