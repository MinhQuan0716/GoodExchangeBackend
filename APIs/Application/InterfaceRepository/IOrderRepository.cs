using Application.InterfaceService;
using Application.ViewModel.OrderModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceRepository
{
    public interface IOrderRepository:IGenericRepository<Order>
    {
        Task<List<ReceiveOrderViewModel>> GetAllOrderByCurrentUserId(Guid userId);
        Task<List<SentOrderViewModel>> GetAllOrderByCreatedByUserId(Guid userId);
        Task<List<Order>> GetOrderByUserIdAndPostId(Guid userId, Guid postId);
        Task<List<Order>> GetOrderByPostId(Guid postId); 
        Task<ReceiveOrderViewModel> GetOrderDetail(Guid orderId);
        Task<List<ReceiveOrderViewModel>> GetAllOrder();
        Task<List<ReceiveOrderViewModel>> GetAllOrderBy2UserId(Guid userId1, Guid userId2);
        Task<List<ReceiveOrderViewModel>> GetAllOrderByUserId(Guid userId);
        Task<List<ReceiveOrderViewModel>> GetAllReceiveOrderBy2UserId(Guid orderCreatedBy, Guid postOwnerId);
        Task<List<SentOrderViewModel>> GetAllSendOrderBy2UserId(Guid orderCreatedBy, Guid postOwnerId);
        Task<List<OrderViewModelForWeb>> GetAllOrderForWeb();
    }
}
