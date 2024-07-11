using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.WalletModel;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class WalletRepository : GenericRepository<Wallet>, IWalletRepository
    {
        private readonly AppDbContext _appDbContext;
        public WalletRepository(AppDbContext appDbContext, IClaimService claimService, ICurrentTime currentTime) : base(appDbContext, claimService, currentTime)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Wallet> FindWalletByUserId(Guid userId)
        {
            return await _appDbContext.Wallets.Where(x => x.OwnerId == userId).SingleOrDefaultAsync();
        }

        public async Task<WalletViewModel> GetWalletByUserId(Guid userId)
        {
           var userWallet= await _appDbContext.Wallets.Where(x=>x.OwnerId==userId)
                                              .Select(x=>new WalletViewModel
                                              {
                                                  Id = x.Id,
                                                  Email=x.Owner.Email,
                                                  Username=x.Owner.UserName,
                                                  UserBalance=x.UserBalance
                                              }).SingleAsync();
            if(userWallet!=null)
            {
                return userWallet;
            }
            return null;
        }
    }
}
