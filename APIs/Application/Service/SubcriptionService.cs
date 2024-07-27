using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.SubcriptionModel;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class SubcriptionService : ISubcriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClaimService _claimService;
        public SubcriptionService(IUnitOfWork unitOfWork,IMapper mapper, IClaimService claimService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _claimService = claimService;
        }

        public async Task<bool> CreateSubcription(CreateSubcriptionModel createSubcriptionModel)
        {
            var subcription = _mapper.Map<Subcription>(createSubcriptionModel);
            await _unitOfWork.SubcriptionRepository.AddAsync(subcription);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> ExtendSubscription()
        {
           var listUser=await _unitOfWork.UserRepository.GetAllMember();
            foreach(var user in listUser)
            {
                var userWallet=await _unitOfWork.WalletRepository.GetWalletByUserId(user.Id);
                var wallet=await _unitOfWork.WalletRepository.GetByIdAsync(userWallet.Id);
                var subscriptionHistoriesViewModel=await _unitOfWork.SubscriptionHistoryRepository.GetCurrentUserAvailableSubscripion(user.Id);
                foreach(var subscriptionHistoryViewModel in subscriptionHistoriesViewModel)
                {
                    var subscription = await _unitOfWork.SubcriptionRepository.GetByIdAsync(subscriptionHistoryViewModel.SubscriptionId);
                    var subscriptionHistory = await _unitOfWork.SubscriptionHistoryRepository.GetByIdAsync(subscriptionHistoryViewModel.Id);
                    if (subscriptionHistory.EndDate > DateTime.UtcNow)
                    {
                        if (wallet.UserBalance < subscription.Price)
                        {
                            subscriptionHistory.Status = false;
                            _unitOfWork.SubscriptionHistoryRepository.Update(subscriptionHistory);
                        }
                        else
                        {
                            wallet.UserBalance = wallet.UserBalance - subscription.Price;
                            subscriptionHistory.EndDate = DateTime.UtcNow.AddMonths((int)subscription.ExpiryMonth);
                            _unitOfWork.WalletRepository.Update(wallet);
                            _unitOfWork.SubscriptionHistoryRepository.Update(subscriptionHistory);
                            WalletTransaction newTransaction = new WalletTransaction()
                            {
                                WalletId= wallet.Id,    
                                TransactionType=$"Extend subscription for {subscription.Description}",
                                SubscriptionId =subscription.Id,
                            };
                            _unitOfWork.WalletTransactionRepository.AddAsync(newTransaction);
                        }
                    }
                }
            }
            return await _unitOfWork.SaveChangeAsync()>0;
        }

        public async Task<List<Subcription>> GetAllSubscriptionAsync()
        {
          return await _unitOfWork.SubcriptionRepository.GetAllAsync();
        }

        public async Task<bool> DeactiveSubscriptionAsync(Guid subscriptionId)
        {
            var subscription=await _unitOfWork.SubcriptionRepository.GetByIdAsync(subscriptionId);
            if(subscription == null)
            {
                return false;
            }
            _unitOfWork.SubcriptionRepository.SoftRemove(subscription);
           return await _unitOfWork.SaveChangeAsync()>0;
        }

        public async Task<bool> RevokeSubscriptionAsync(Guid subscriptionId)
        {
            var subscription=await _unitOfWork.SubcriptionRepository.GetSubscriptionForRevokeAsync(subscriptionId);
            if(subscription == null)
            {
                return false;
            }
            subscription.IsDelete = false;
            _unitOfWork.SubcriptionRepository.Update(subscription);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> UpdateSubcription(UpdateSubscriptionModel updateSubcriptionModel)
        {
            var foundSubscription = await _unitOfWork.SubcriptionRepository.GetByIdAsync(updateSubcriptionModel.Id);
            if(foundSubscription == null)
            {
                return false;
            }
            _mapper.Map(updateSubcriptionModel,foundSubscription,typeof(UpdateSubscriptionModel),typeof(Subcription));
            _unitOfWork.SubcriptionRepository.Update(foundSubscription);
            return await _unitOfWork.SaveChangeAsync()>0;
        }
    }
}
