using Application.InterfaceService;
using Application.ViewModel.RequestModel;
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
        Task<List<ReceiveOrderViewModel>> GetAllRequestByCurrentUserId(Guid userId);
        Task<List<SentOrderViewModel>> GetAllRequestByCreatedByUserId(Guid userId);
        Task<List<Order>> GetRequestByUserIdAndPostId(Guid userId, Guid postId);
        Task<List<Order>> GetRequestByPostId(Guid postId); 
        Task<ReceiveOrderViewModel> GetOrderDetail(Guid orderId);
        Task<List<ReceiveOrderViewModel>> GetAllOrder();
    }
}
