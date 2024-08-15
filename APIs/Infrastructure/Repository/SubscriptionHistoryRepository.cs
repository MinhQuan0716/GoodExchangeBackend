using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.SubcriptionModel;
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
    public class SubscriptionHistoryRepository : GenericRepository<SubscriptionHistory>, ISubscriptionHistoryRepository
    {
        private readonly AppDbContext _appDbContext;
        public SubscriptionHistoryRepository(AppDbContext appDbContext, IClaimService claimService, ICurrentTime currentTime) : base(appDbContext, claimService, currentTime)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<SubscriptionHistoryViewModel>> GetAllSubscriptionHistory()
        {
           var subscriptionHistoryList=await _appDbContext.SubscriptionHistories.Where(x=>x.IsDelete==false)
                                                                               .Include(x=>x.User).AsSplitQuery()
                                                                               .Include(x=>x.Subcription).AsSplitQuery()
                                                                               .Select(x=>new SubscriptionHistoryViewModel
                                                                               {
                                                                                   Email=x.User.Email,
                                                                                   UsertName=x.User.UserName,
                                                                                   StartDate=x.StartDate,
                                                                                   EndDate=x.EndDate,
                                                                                   Status=x.Status? "Available":"Expried",
                                                                                   subcriptionModel = new SubscriptionDetailViewModel
                                                                                   {
                                                                                       SubscriptionId = x.SubcriptionId,
                                                                                       Description =x.Subcription.Description,
                                                                                       ExpiryMonth=x.Subcription.ExpiryMonth,
                                                                                       Price = x.Subcription.Price,
                                                                                       SubcriptionType = x.Subcription.SubcriptionType
                                                                                   }
                                                                               }).AsQueryable().AsNoTracking().ToListAsync();
            return subscriptionHistoryList;
        }

        public async Task<List<SubscriptionHistoryDetailViewModel>> GetCurrentUserAvailableSubscripion(Guid userId)
        {
            int postAmount = _appDbContext.Posts.Where(x => x.UserId == userId && x.IsDelete == false).ToList().Count();
            var listUserSubscription = await _appDbContext.SubscriptionHistories.Where(x => x.UserId == userId && x.IsDelete == false && x.Status == true)
                                                                               .Include(x => x.Subcription).AsSplitQuery()
                                                                              .Select(x => new SubscriptionHistoryDetailViewModel
                                                                              {
                                                                                  StartDate = x.StartDate,
                                                                                  EndDate = x.EndDate,
                                                                                  Status = x.Status ? "Available" : "Expired",
                                                                                  SubscriptionId = x.Subcription.Id,
                                                                                  Id = x.Id,
                                                                                  subcriptionModel = new SubscriptionDetailViewModel
                                                                                  {
                                                                                      SubscriptionId = x.SubcriptionId,
                                                                                      Description = x.Subcription.Description,
                                                                                      ExpiryMonth = x.Subcription.ExpiryMonth,
                                                                                      Price = x.Subcription.Price,
                                                                                      SubcriptionType = x.Subcription.SubcriptionType
                                                                                  },
                                                                                  PostAmount = postAmount
                                                                              }).ToListAsync();
            return listUserSubscription;
        }

        public async Task<List<SubscriptionHistory>> GetLastSubscriptionByUserIdAsync(Guid userId)
        {
            var subscription = await _appDbContext.SubscriptionHistories.Where(x => x.UserId == userId&&x.IsDelete==false)
                                                                       .ToListAsync();
            return subscription;
        }

        public async Task<List<SubscriptionHistory>> GetUserExpireSubscription(Guid userId)
        {
            var listExpireSubscription = await _appDbContext.SubscriptionHistories.Where(x => x.UserId == userId && x.IsDelete == false && x.Status == false)
                                                                                .ToListAsync();
            return listExpireSubscription;
        }    

        public async Task<List<SubscriptionHistoryDetailViewModel>> GetUserPurchaseSubscription(Guid userId)
        {
            int postAmount=_appDbContext.Posts.Where(x=>x.UserId== userId).ToList().Count();
            var listUserSubscription = await _appDbContext.SubscriptionHistories.Where(x => x.UserId == userId && x.IsDelete == false)
                                                                             .Include(x => x.Subcription).AsSplitQuery()
                                                                             .Select(x => new SubscriptionHistoryDetailViewModel
                                                                             {
                                                                                 Id = x.Id,
                                                                                 StartDate = x.StartDate,
                                                                                 EndDate = x.EndDate,
                                                                                 Status = x.Status ? "Available" : "Expried",
                                                                                 SubscriptionId = x.Subcription.Id,
                                                                                 subcriptionModel = new SubscriptionDetailViewModel
                                                                                 {
                                                                                     SubscriptionId = x.SubcriptionId,
                                                                                     Description = x.Subcription.Description,
                                                                                     ExpiryMonth = x.Subcription.ExpiryMonth,
                                                                                     Price = x.Subcription.Price,
                                                                                     SubcriptionType = x.Subcription.SubcriptionType
                                                                                 },
                                                                                 PostAmount = postAmount
                                                                             }).ToListAsync();
            return listUserSubscription;
        }
    }
}
