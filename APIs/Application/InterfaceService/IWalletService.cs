using Application.ViewModel.WalletModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceService
{
    public interface IWalletService
    {
        public Task<WalletViewModel> GetWalletByUserIdAsync();
    }
}
