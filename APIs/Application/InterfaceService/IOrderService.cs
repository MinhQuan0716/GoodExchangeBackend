using Application.ViewModel.OrderModel;
using Application.ViewModel.OrderModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceService
{
    public interface IOrderService
    {
        Task<List<ReceiveOrderViewModel>> GetAllOrdersOfCurrentUserAsync();
        Task<List<SentOrderViewModel>> GetAllOrdersOfCreatebByUserAsync();
        Task<bool> AcceptOrder(Guid OrderId);
        Task<bool> CheckOrderStatusByPostId(Guid postId); 
        Task<ReceiveOrderViewModel> GetOrderDetailAsync(Guid postId);
        Task<bool> DeliveredOrder(Guid orderId);
        Task<bool> CancleOrder(Guid orderId);
        Task<bool> ConfirmOrder(Guid orderId);
        Task<bool> CancleOrderForAdmin(Guid orderId);
        Task<List<ReceiveOrderViewModel>> GetAllOrderAsync();
        Task<List<ReceiveOrderViewModel>> GetAllOrderByChatRoomId(Guid chatRoomId);
        Task<List<ReceiveOrderViewModel>> GetAllOrderByCurrentUser();
        Task<List<SentOrderViewModel>> GetSendOrderByChatRoomId(Guid chatRoomId);
        Task<List<ReceiveOrderViewModel>> GetReceiveOrderByChatRoomId(Guid chatRoomId);
    }
}
