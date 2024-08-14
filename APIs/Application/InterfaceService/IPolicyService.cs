using Application.ViewModel.PolicyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceService
{
    public interface IPolicyService
    {
        Task<PostPriceViewModel> GetPostPrice();
        Task<OrderCancelledTimeViewModel> GetOrderCancelledTime();
        Task<bool> UpdatePostPrice(PostPriceViewModel postPriceViewModel);
        Task<bool> UpdateOrderCancelledTime(OrderCancelledTimeViewModel orderCancelledTimeViewModel);
    }
}
