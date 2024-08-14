using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.TransactionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class WalletTransactionService : IWalletTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimService _claimService;
        public WalletTransactionService(IUnitOfWork unitOfWork, IClaimService claimService)
        {
            _unitOfWork = unitOfWork;
            _claimService = claimService;
        }
        public async Task<List<TransactionViewModelForWeb>> GetAllTransactionAsync()
        {
            return await _unitOfWork.WalletTransactionRepository.GetAllTransaction();
        }
        public async Task<List<TransactionViewModel>> GetAllTransactionByCurrentUserIdAsync()
        {
            return await _unitOfWork.WalletTransactionRepository.GetAllTransactionByUserId(_claimService.GetCurrentUserId);
        }
    }
}
