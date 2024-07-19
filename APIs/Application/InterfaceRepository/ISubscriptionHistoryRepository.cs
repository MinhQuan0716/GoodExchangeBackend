using Application.ViewModel.SubscriptionHistoryModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceRepository
{
    public interface ISubscriptionHistoryRepository:IGenericRepository<SubcriptionHistory>
    {
        Task<List<SubcriptionHistory>> GetLastSubscriptionByUserIdAsync(Guid userId);
        Task<List<SubscriptionHistoryViewModel>> GetAllSubscriptionHistory();
        Task<List<SubscriptionHistoryDetailViewModel>> GetUserPruchaseSubscription(Guid userId);
        Task<List<SubscriptionHistoryDetailViewModel>> GetCurrentUserAvailableSubscripion(Guid userId);
    }
}
