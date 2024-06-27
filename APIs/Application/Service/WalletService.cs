using Application.InterfaceService;
using Application.ViewModel.WalletModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimService _claimService;
        public WalletService(IUnitOfWork unitOfWork, IClaimService claimService)
        {
            _unitOfWork = unitOfWork;
            _claimService = claimService;
        }
        public async Task<WalletViewModel> GetWalletByUserIdAsync()
        {
            return await _unitOfWork.WalletRepository.GetWalletByUserId(_claimService.GetCurrentUserId);
        }
    }
}
