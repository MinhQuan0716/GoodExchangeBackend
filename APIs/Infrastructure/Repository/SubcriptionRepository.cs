using Application.InterfaceRepository;
using Application.InterfaceService;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class SubcriptionRepository : GenericRepository<Subcription>, ISubcriptionRepository
    {
        private readonly AppDbContext _appDbContext;
        public SubcriptionRepository(AppDbContext appDbContext, IClaimService claimService, ICurrentTime currentTime) : base(appDbContext, claimService, currentTime)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<Subcription>> GetAllDeactiveSubscription()
        {
            var listSubscription = await _appDbContext.Subcriptions.ToListAsync();
            return listSubscription;
        }

        public async Task<Subcription> GetSubscriptionForRevokeAsync(Guid subscriptionId)
        {
            return await _appDbContext.Subcriptions.Where(x=>x.Id==subscriptionId).SingleAsync();
        }
    }
}
