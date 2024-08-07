using Application.ViewModel.PostModel;
using Application.ViewModel.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.OrderModel
{
    public class ReceiveOrderViewModel
    {
        public Guid OrderId { get; set; }
        public string OrderMessage { get; set; }
        public string OrderStatus { get; set; }
        public DateTime? CreationDate { get; set; }
        public PostViewModelForOrder Post { get; set; }
        public UserViewModelForOrder User{ get; set; }
    }
}
