using Application.ViewModel.SubscriptionHistoryModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceRepository
{
    public interface ISubscriptionHistoryRepository:IGenericRepository<SubscriptionHistory>
    {
        Task<List<SubscriptionHistory>> GetLastSubscriptionByUserIdAsync(Guid userId);
        Task<List<SubscriptionHistoryViewModel>> GetAllSubscriptionHistory();
        Task<List<SubscriptionHistoryDetailViewModel>> GetUserPurchaseSubscription(Guid userId);
        Task<List<SubscriptionHistoryDetailViewModel>> GetCurrentUserAvailableSubscripion(Guid userId);
        Task<List<SubscriptionHistory>> GetUserExpireSubscription(Guid userId);
    }
}
