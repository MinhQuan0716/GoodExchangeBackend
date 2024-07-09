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
    public class SubscriptionHistoryRepository : GenericRepository<SubcriptionHistory>, ISubscriptionHistoryRepository
    {
        private readonly AppDbContext _appDbContext;
        public SubscriptionHistoryRepository(AppDbContext appDbContext, IClaimService claimService, ICurrentTime currentTime) : base(appDbContext, claimService, currentTime)
        {
            _appDbContext = appDbContext;
        }

        public async Task<SubcriptionHistory> GetLastSubscriptionByUserIdAsync(Guid userId)
        {
            var subscription = await _appDbContext.SubcriptionHistories.Where(x => x.UserId == userId)
                                                                             .OrderBy(x=>x.CreationDate)
                                                                             .LastAsync();
            return subscription;
        }
    }
}
