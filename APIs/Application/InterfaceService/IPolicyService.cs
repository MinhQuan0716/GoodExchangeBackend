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
        Task<List<PostPriceViewModel>> GetPostPrice();
        Task<List<OrderCancelledTimeViewModel>> GetOrderCancelledTime();
        Task<bool> UpdatePostPrice(UpdatePostPriceModel updatePostPriceModel);
        Task<bool> UpdateOrderCancelledTime(UpdateOrderCancelledTimeModel updateOrderCancelledTimeModel);
    }
}
