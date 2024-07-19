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
            return await _unitOfWork.SubscriptionHistoryRepository.GetUserPruchaseSubscription(_claimService.GetCurrentUserId);
        }

        public async Task<List<SubscriptionHistoryDetailViewModel>> GetCurrentUsersAvailableSubscription()
        {
           return await _unitOfWork.SubscriptionHistoryRepository.GetCurrentUserAvailableSubscripion(_claimService.GetCurrentUserId);
        }
    }
}
