using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class WalletTransactionRepository : GenericRepository<WalletTransaction>, IWalletTransactionRepository
    {
        private readonly AppDbContext _appDbContext;
        public WalletTransactionRepository(AppDbContext appDbContext, IClaimService claimService, ICurrentTime currentTime) : base(appDbContext, claimService, currentTime)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<TransactionViewModel>> GetAllTransaction()
        {
            var listTransaction = await _appDbContext.WalletTransactions.Where(x => x.IsDelete == false)
                                                                       .Include(x => x.Wallet)
                                                                       .ThenInclude(wallet => wallet.Owner).AsSplitQuery()
                                                                       .Select(x => new TransactionViewModel
                                                                       {
                                                                           Id=x.Id,
                                                                           Username=x.Wallet.Owner.UserName,
                                                                           Email=x.Wallet.Owner.Email,
                                                                           Action=x.TransactionType,
                                                                           Amount=x.Amount
                                                                       }).ToListAsync();
            return listTransaction;
        }

        public async Task<List<TransactionViewModel>> GetAllTransactionByUserId(Guid userId)
        {
            var listTransaction = await _appDbContext.WalletTransactions.Where(x => x.IsDelete == false)
                                                                       .Include(x => x.Wallet).ThenInclude(wallet => wallet.Owner).AsSplitQuery()
                                                                       .Where(x => x.Wallet.OwnerId == userId)
                                                                       .Select(x => new TransactionViewModel
                                                                       {
                                                                           Username = x.Wallet.Owner.UserName,
                                                                           Email = x.Wallet.Owner.Email,
                                                                           Action = x.TransactionType,
                                                                           Amount = x.Amount
                                                                       }).ToListAsync();
            return listTransaction;
        }

        public async Task<WalletTransaction> GetByOrderIdAsync(Guid orderId)
        {
            var Transaction = await _appDbContext.WalletTransactions
                                    .Where(x => x.IsDelete == false)
                                    .Where(x => x.OrderId == orderId)
                                    .FirstOrDefaultAsync();
            return Transaction;
        }

        public async Task<Guid> GetLastSaveWalletTransactionId()
        {
            var lasSaveWalletTransaction =  await _appDbContext.WalletTransactions.Where(x => x.IsDelete == false)
                                                         .OrderBy(x => x.CreationDate)
                                                         .LastAsync();
            return lasSaveWalletTransaction.Id;
        }
    }
}
