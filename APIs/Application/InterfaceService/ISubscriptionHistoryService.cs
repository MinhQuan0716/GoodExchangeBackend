using Application.ViewModel.SubscriptionHistoryModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceService
{
    public interface ISubscriptionHistoryService
    {
        Task<List<SubscriptionHistoryViewModel>> GetAllSubscriptionHistoriesAsync();
        Task<List<SubscriptionHistoryDetailViewModel>> GetAllUsersSubscriptionHistoryDetailAsync();
        Task<List<SubscriptionHistoryDetailViewModel>> GetCurrentUsersAvailableSubscription();
    }
}
