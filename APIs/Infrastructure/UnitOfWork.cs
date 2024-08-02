using Application;
using Application.InterfaceRepository;
using Application.InterfaceService;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserRepository _userRepository;
        private readonly IExchangeConditionRepository _exchangeConditionRepository;
        private readonly IPostRepository _postRepository;
        private readonly IProductRepository _productRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IVerifyUsersRepository _verifyUsersRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWishListRepository _wishListRepository;
        private readonly ISubcriptionRepository _subcriptionRepository;
        private readonly ISubscriptionHistoryRepository _subscriptionHistoryRepository;
        private readonly IRatingRepository _ratingRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IChatRoomRepository _chatRoomRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IReportRepository _reportRepository;
        public UnitOfWork(IUserRepository userRepository, AppDbContext dbContext, 
            IPostRepository postRepository, IProductRepository productRepository, IWalletRepository walletRepository, 
            IVerifyUsersRepository verifyUsersRepository, IExchangeConditionRepository exchangeConditionRepository,
            ICategoryRepository categoryRepository,IWishListRepository wishListRepository,ISubcriptionRepository subcriptionRepository,
            IRatingRepository ratingRepository, IMessageRepository messageRepository, IOrderRepository requestRepository, 
            IChatRoomRepository chatRoomRepository,ISubscriptionHistoryRepository subscriptionHistoryRepository,IWalletTransactionRepository walletTransactionRepository,
            IOrderRepository orderRepository,IReportRepository reportRepository)
        {
            _userRepository = userRepository;
            _dbContext = dbContext;
            _postRepository = postRepository;
            _productRepository = productRepository;
            _walletRepository = walletRepository;
            _verifyUsersRepository = verifyUsersRepository;
            _exchangeConditionRepository = exchangeConditionRepository;
            _categoryRepository = categoryRepository;
            _wishListRepository = wishListRepository;
            _subcriptionRepository = subcriptionRepository;
            _ratingRepository = ratingRepository;
            _messageRepository = messageRepository; 
            _chatRoomRepository = chatRoomRepository;
            _subscriptionHistoryRepository=subscriptionHistoryRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _orderRepository= orderRepository;  
            _reportRepository=reportRepository;
        }

        public IUserRepository UserRepository =>_userRepository;


        public IPostRepository PostRepository => _postRepository;

        public IProductRepository ProductRepository => _productRepository;

        public IWalletRepository WalletRepository => _walletRepository;
        public IVerifyUsersRepository VerifyUsersRepository => _verifyUsersRepository;

        public IExchangeConditionRepository ExchangeConditionRepository => _exchangeConditionRepository;

        public ICategoryRepository CategoryRepository => _categoryRepository;

        public IWishListRepository WishListRepository => _wishListRepository;

        public ISubcriptionRepository SubcriptionRepository => _subcriptionRepository;

        public IRatingRepository RatingRepository => _ratingRepository;

        public IMessageRepository MessageRepository => _messageRepository;


        public IChatRoomRepository ChatRoomRepository => _chatRoomRepository;

        public ISubscriptionHistoryRepository SubscriptionHistoryRepository => _subscriptionHistoryRepository;

        public IWalletTransactionRepository WalletTransactionRepository => _walletTransactionRepository;

        public IOrderRepository OrderRepository => _orderRepository;

        public IReportRepository ReportRepository => _reportRepository;

        public Task<int> SaveChangeAsync()
        {
            return _dbContext.SaveChangesAsync();
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _dbContext.Database.BeginTransactionAsync();
        }
    }
}
