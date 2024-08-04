using Application.ViewModel.SubcriptionModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceRepository
{
    public interface ISubcriptionRepository:IGenericRepository<Subscription>
    {
        Task<List<Subscription>> GetAllDeactiveSubscription();
        Task<Subscription> GetSubscriptionForRevokeAsync(Guid subscriptionId);
    }
}
