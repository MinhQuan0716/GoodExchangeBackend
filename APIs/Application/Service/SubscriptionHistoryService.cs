using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.SubscriptionHistoryModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class SubscriptionHistoryService : ISubscriptionHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimService _claimService;
        public SubscriptionHistoryService(IUnitOfWork unitOfWork, IClaimService claimService)
        {
            _unitOfWork = unitOfWork;
            _claimService = claimService;
        }
        public async Task<List<SubscriptionHistoryViewModel>> GetAllSubscriptionHistoriesAsync()
        {
            return await _unitOfWork.SubscriptionHistoryRepository.GetAllSubscriptionHistory();
        }

        public async Task<List<SubscriptionHistoryDetailViewModel>> GetAllUsersSubscriptionHistoryDetailAsync()
        {
            return await _unitOfWork.SubscriptionHistoryRepository.GetUserPurchaseSubscription(_claimService.GetCurrentUserId);
        }

        public async Task<List<SubscriptionHistoryDetailViewModel>> GetCurrentUsersAvailableSubscription()
        {
           return await _unitOfWork.SubscriptionHistoryRepository.GetCurrentUserAvailableSubscripion(_claimService.GetCurrentUserId);
        }

        public async Task<bool> UnsubscribeSubscription(Guid subscriptionId)
        {
            bool isRemove = false;
            var listSubscriptionHistories = await _unitOfWork.SubscriptionHistoryRepository.GetAllAsync();
            var subscriptionHistoryToDeactive = listSubscriptionHistories.Where(x => x.UserId == _claimService.GetCurrentUserId && x.SubcriptionId == subscriptionId).ToList();
            var wallet =await _unitOfWork.WalletRepository.GetUserWalletByUserId(_claimService.GetCurrentUserId);
            var subscription = await _unitOfWork.SubcriptionRepository.GetByIdAsync(subscriptionId);
            var listPost=await _unitOfWork.PostRepository.GetAllPostsByCreatedByIdAsync(_claimService.GetCurrentUserId);
            if(subscriptionHistoryToDeactive == null)
            {
                throw new Exception("You already unsubscribe this subscription");
            }
            if (listPost.Count()==0)
            {
                foreach(var subscriptionHistory in subscriptionHistoryToDeactive)
                {
                    subscriptionHistory.Status = false;
                    wallet.UserBalance += subscription.Price;
                    _unitOfWork.SubscriptionHistoryRepository.Update(subscriptionHistory);
                    _unitOfWork.WalletRepository.Update(wallet);
                    isRemove= await _unitOfWork.SaveChangeAsync() > 0;
                }
                
            }
            foreach (var subscriptionHistory in subscriptionHistoryToDeactive)
            {
                subscriptionHistory.Status = false;
                wallet.UserBalance += subscription.Price;
                _unitOfWork.SubscriptionHistoryRepository.Update(subscriptionHistory);
                _unitOfWork.WalletRepository.Update(wallet);
                isRemove= await _unitOfWork.SaveChangeAsync() > 0;
            }
            return isRemove;
        }
    }
}
