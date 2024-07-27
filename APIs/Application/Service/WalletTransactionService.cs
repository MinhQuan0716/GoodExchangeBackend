using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel;
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
        public WalletTransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<TransactionViewModel>> GetAllTransactionAsync()
        {
            return await _unitOfWork.WalletTransactionRepository.GetAllTransaction();
        }
    }
}
