using Application.InterfaceRepository;
using Microsoft.EntityFrameworkCore.Storage;
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
        public IWishListRepository WishListRepository { get; }
        public ISubcriptionRepository SubcriptionRepository { get; }
        public ISubscriptionHistoryRepository SubscriptionHistoryRepository { get; }
        public IRatingRepository RatingRepository { get; }
        public IMessageRepository MessageRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IChatRoomRepository ChatRoomRepository { get; }
        public IWalletTransactionRepository WalletTransactionRepository { get; }
        public IReportRepository ReportRepository { get; }
        public Task<int> SaveChangeAsync();

    }
}
