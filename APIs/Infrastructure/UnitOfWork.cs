using Application;
using Application.InterfaceRepository;
using Application.InterfaceService;
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
        private readonly IRatingRepository _ratingRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IRequestRepository _requestRepository;
        public UnitOfWork(IUserRepository userRepository, AppDbContext dbContext, 
            IPostRepository postRepository, IProductRepository productRepository, IWalletRepository walletRepository, 
            IVerifyUsersRepository verifyUsersRepository, IExchangeConditionRepository exchangeConditionRepository,
            ICategoryRepository categoryRepository,IWishListRepository wishListRepository,ISubcriptionRepository subcriptionRepository,
            IRatingRepository ratingRepository, IMessageRepository messageRepository,IRequestRepository requestRepository)
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
            _requestRepository = requestRepository;
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

        public IRequestRepository RequestRepository => _requestRepository;

        public Task<int> SaveChangeAsync()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}
