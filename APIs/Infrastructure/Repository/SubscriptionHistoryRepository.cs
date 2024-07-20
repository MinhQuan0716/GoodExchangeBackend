using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.SubscriptionHistoryModel;
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

        public async Task<List<SubscriptionHistoryViewModel>> GetAllSubscriptionHistory()
        {
           var subscriptionHistoryList=await _appDbContext.SubcriptionHistories.Where(x=>x.IsDelete==false)
                                                                               .Include(x=>x.User).AsSplitQuery()
                                                                               .Include(x=>x.Subcription).AsSplitQuery()
                                                                               .Select(x=>new SubscriptionHistoryViewModel
                                                                               {
                                                                                   Email=x.User.Email,
                                                                                   UsertName=x.User.UserName,
                                                                                   StartDate=DateOnly.FromDateTime(x.StartDate),
                                                                                   EndDate=DateOnly.FromDateTime(x.EndDate),
                                                                                   Status=x.Status? "Available":"Expried",
                                                                               }).AsQueryable().AsNoTracking().ToListAsync();
            return subscriptionHistoryList;
        }

        public async Task<List<SubscriptionHistoryDetailViewModel>> GetCurrentUserAvailableSubscripion(Guid userId)
        {
            var listUserSubscription = await _appDbContext.SubcriptionHistories.Where(x => x.UserId == userId && x.IsDelete == false&&x.Status==true)
                                                                               .Include(x=>x.Subcription).AsSplitQuery()
                                                                              .Select(x => new SubscriptionHistoryDetailViewModel
                                                                              {
                                                                               StartDate = DateOnly.FromDateTime(x.StartDate),
                                                                               EndDate = DateOnly.FromDateTime(x.EndDate),
                                                                               Status = x.Status ? "Available":"Expired",
                                                                               SubscriptionId=x.Subcription.Id,
                                                                               Id=x.Id
                                                                               }).ToListAsync();
            return listUserSubscription;
        }

        public async Task<List<SubcriptionHistory>> GetLastSubscriptionByUserIdAsync(Guid userId)
        {
            var subscription = await _appDbContext.SubcriptionHistories.Where(x => x.UserId == userId&&x.IsDelete==false)
                                                                       .ToListAsync();
            return subscription;
        }

        public async Task<List<SubscriptionHistoryDetailViewModel>> GetUserPruchaseSubscription(Guid userId)
        {
           var listUserSubscription=await _appDbContext.SubcriptionHistories.Where(x=>x.UserId== userId&&x.IsDelete==false)
                                                                            .Include(x=>x.Subcription).AsSplitQuery()
                                                                            .Select(x=>new SubscriptionHistoryDetailViewModel
                                                                            {
                                                                                Id = x.Id,
                                                                                StartDate=DateOnly.FromDateTime(x.StartDate),
                                                                                EndDate=DateOnly.FromDateTime(x.EndDate),
                                                                                Status=x.Status?"Available":"Expried",
                                                                                SubscriptionId=x.Subcription.Id,
                                                                            }).ToListAsync();
            return listUserSubscription;
        }
    }
}
