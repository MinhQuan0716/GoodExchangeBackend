using Application.ViewModel.SubcriptionModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceService
{
    public interface ISubcriptionService
    {
        Task<bool> CreateSubcription(CreateSubcriptionModel createSubcriptionModel);
        Task<bool> UpdateSubcription(UpdateSubscriptionModel updateSubcriptionModel);
        Task<List<Subscription>> GetAllSubscriptionAsync();
        Task<bool> ExtendSubscription();
        Task<bool> DeactiveSubscriptionAsync(Guid subscriptionId);
        Task<bool> RevokeSubscriptionAsync(Guid subscriptionId);
        Task<SubscriptionDetailViewModel> GetSubscriptionDetailAsync(Guid subscriptionId);
        Task<bool> PrioritySubscriptionAsync(Guid subscriptionId);
        Task<bool> UnPrioritySubscriptionAsync(Guid subscriptionId);
    }
}
