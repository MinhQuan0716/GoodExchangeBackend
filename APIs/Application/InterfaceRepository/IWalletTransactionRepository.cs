using Application.ViewModel.TransactionModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceRepository
{
    public interface IWalletTransactionRepository:IGenericRepository<WalletTransaction>
    {
        Task<Guid> GetLastSaveWalletTransactionId();
        Task<List<TransactionViewModelForWeb>> GetAllTransaction();
        Task<List<TransactionViewModel>> GetAllTransactionByUserId(Guid userId);
        Task<WalletTransaction> GetByOrderIdAsync(Guid orderId);
    }
}
