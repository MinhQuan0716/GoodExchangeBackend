using Application.ViewModel.WalletModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceRepository
{
    public  interface IWalletRepository:IGenericRepository<Wallet>
    {
        Task<Wallet> FindWalletByUserId(Guid userId);
        Task<WalletViewModel> GetWalletByUserId (Guid userId);
    }
}
