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
            var subcription = _mapper.Map<Subscription>(createSubcriptionModel);
            subcription.Description = "Standard";
            subcription.ExpiryDay = createSubcriptionModel.ExpiryDay;
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
                    if (subscriptionHistory.IsExtend == false)
                    {
                    }
                    if (subscriptionHistory.EndDate < DateTime.UtcNow)
                    {
                    }
                    else
                    {
                        if (wallet.UserBalance < subscription.Price)
                        {
                            WalletTransaction walletTransaction = new WalletTransaction()
                            {
                                TransactionType="Extend subscription failed,user balance not enough",
                                WalletId=wallet.Id
                            };
                             subscriptionHistory.Status = false;
                            _unitOfWork.SubscriptionHistoryRepository.Update(subscriptionHistory);
                            _unitOfWork.WalletTransactionRepository.AddAsync(walletTransaction);
                        } else
                        {
                            wallet.UserBalance=wallet.UserBalance-subscription.Price;
                            WalletTransaction walletTransaction = new WalletTransaction()
                            {
                                TransactionType = "Extend subscription succees",
                                WalletId=wallet.Id,
                                Amount=subscription.Price
                            };
                            subscriptionHistory.Status = true;
                            _unitOfWork.SubscriptionHistoryRepository.Update(subscriptionHistory);
                            _unitOfWork.WalletTransactionRepository.AddAsync(walletTransaction);
                            _unitOfWork.WalletRepository.Update(wallet);
                        }
                    }
                }
            }
            return await _unitOfWork.SaveChangeAsync()>0;
        }

        public async Task<List<Subscription>> GetAllSubscriptionAsync()
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
            var desc = foundSubscription.Description;
            _mapper.Map(updateSubcriptionModel,foundSubscription,typeof(UpdateSubscriptionModel),typeof(Subscription));
            foundSubscription.Description = desc;
            _unitOfWork.SubcriptionRepository.Update(foundSubscription);
            return await _unitOfWork.SaveChangeAsync()>0;
        }

        public async Task<SubscriptionDetailViewModel> GetSubscriptionDetailAsync(Guid subscriptionId)
        {
            var subscriptionDetail=await _unitOfWork.SubcriptionRepository.GetByIdAsync(subscriptionId);
            var subscriptionDetailViewModel=_mapper.Map<SubscriptionDetailViewModel>(subscriptionDetail);
            return subscriptionDetailViewModel;
        }

        public async Task<bool> PrioritySubscriptionAsync(Guid subscriptionId)
        {
            var subscriptionDetail = await _unitOfWork.SubcriptionRepository.GetByIdAsync(subscriptionId);
            subscriptionDetail.Description = "Priority";
            _unitOfWork.SubcriptionRepository.Update(subscriptionDetail);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> UnPrioritySubscriptionAsync(Guid subscriptionId)
        {
            var subscriptionDetail = await _unitOfWork.SubcriptionRepository.GetByIdAsync(subscriptionId);
            subscriptionDetail.Description = "Standard";
            _unitOfWork.SubcriptionRepository.Update(subscriptionDetail);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }
    }
}
