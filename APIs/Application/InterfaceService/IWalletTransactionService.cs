using Application.ViewModel.TransactionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceService
{
    public interface IWalletTransactionService
    {
        Task<List<TransactionViewModelForWeb>> GetAllTransactionAsync();
        Task<List<TransactionViewModel>> GetAllTransactionByCurrentUserIdAsync();
    }
}
