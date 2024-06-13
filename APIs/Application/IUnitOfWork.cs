using Application.InterfaceRepository;
using Application.InterfaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface IUnitOfWork
    {
        public IUserRepository UserRepository { get;}
        public IPostRepository PostRepository { get;}
        public IProductRepository ProductRepository { get;}
        public IWalletRepository WalletRepository { get;}
        public IVerifyUsersRepository VerifyUsersRepository { get;}
        public IExchangeConditionRepository ExchangeConditionRepository { get;}
        public ICategoryRepository CategoryRepository { get; }
        public Task<int> SaveChangeAsync();

    }
}
