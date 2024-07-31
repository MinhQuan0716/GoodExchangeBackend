using Application.ViewModel.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceService
{
    public interface IOrderService
    {
        Task<bool>SendRequest(CreateOrderModel requestModel);
        Task<List<ReceiveOrderViewModel>> GetAllRequestsOfCurrentUserAsync();
        Task<List<SentOrderViewModel>> GetAllRequestsOfCreatebByUserAsync();
        Task<bool> AcceptRequest(Guid requestId);
        Task<bool> CheckOrderStatusByPostId(Guid postId); 
        Task<ReceiveOrderViewModel> GetOrderDetailAsync(Guid postId);
        Task<bool> DeliveredOrder(Guid orderId);
        Task<bool> CancleOrder(Guid orderId);
        Task<bool> ConfirmOrder(Guid orderId);
        Task<bool> CancleOrderForAdmin(Guid orderId);
        Task<List<ReceiveOrderViewModel>> GetAllOrderAsync();
        Task<List<ReceiveOrderViewModel>> GetAllOrderByChatRoomId(Guid chatRoomId);
        Task<List<ReceiveOrderViewModel>> GetAllOrderByCurrentUser();
    }
}
