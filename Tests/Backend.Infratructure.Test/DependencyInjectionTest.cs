using Application.InterfaceRepository;
using Application.InterfaceService;
using Backend.Domain.Test;
using FluentAssertions;
using Infrastructure;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MobileAPI;
using MobileAPI.MobileService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;
using Application;

namespace Backend.Infratructure.Test
{
    public class DependencyInjectionTest:SetupTest
    {
        private readonly IServiceProvider _serviceProvider;
        public DependencyInjectionTest()
        {
            var service=new ServiceCollection();
            service.AddInfrastructureService("Server=Test;uid=Tester;password=Tested;Database=TestDb;TrustServerCertificate=True;Encrypt=false;MultipleActiveResultSets=true");
            service.AddDbContext<AppDbContext>(
               option => option.UseInMemoryDatabase("test"));
            service.AddMobileAPIService("Test","TestMobile");
            service.AddWebAPIService("Test", "TestWeb");   
            _serviceProvider = service.BuildServiceProvider();
        }
        [Fact]
        public void GetService_ShouldRetunCorrectType()
        {
            var userRepositoryResolved = _serviceProvider.GetRequiredService<IUserRepository>();
            var postRepositoryResolved = _serviceProvider.GetRequiredService<IPostRepository>();
            var productRepositoryResolved = _serviceProvider.GetRequiredService<IProductRepository>();
            var walletRepositoryResolved = _serviceProvider.GetRequiredService<IWalletRepository>();
            var unitOfWorkResolved = _serviceProvider.GetRequiredService<IUnitOfWork>();
            var verifyUsersRepositoryResolved = _serviceProvider.GetRequiredService<IVerifyUsersRepository>();
            var exchangeConditionRepositoryResolved = _serviceProvider.GetRequiredService<IExchangeConditionRepository>();
            var categoryRepositoryResolved = _serviceProvider.GetRequiredService<ICategoryRepository>();
            var wishListRepositoryResolved = _serviceProvider.GetRequiredService<IWishListRepository>();
            var subcriptionRepositoryResolved = _serviceProvider.GetRequiredService<ISubcriptionRepository>();
            var subscriptionHistoryRepositoryResolved = _serviceProvider.GetRequiredService<ISubscriptionHistoryRepository>();
            var ratingRepositoryResolved = _serviceProvider.GetRequiredService<IRatingRepository>();
            var messageRepositoryResolved = _serviceProvider.GetRequiredService<IMessageRepository>();
            var orderRepositoryResolved = _serviceProvider.GetRequiredService<IOrderRepository>();
            var chatRoomRepositoryResolved = _serviceProvider.GetRequiredService<IChatRoomRepository>();
            var walletTransactionRepositoryResolved = _serviceProvider.GetRequiredService<IWalletTransactionRepository>();
            var reportRepositoryResolved = _serviceProvider.GetRequiredService<IReportRepository>();

            userRepositoryResolved.GetType().Should().Be(typeof(UserRepository));
            postRepositoryResolved.GetType().Should().Be(typeof(PostRepository));
            productRepositoryResolved.GetType().Should().Be(typeof(ProductRepository));
            walletRepositoryResolved.GetType().Should().Be(typeof(WalletRepository));
            unitOfWorkResolved.GetType().Should().Be(typeof(UnitOfWork));
            verifyUsersRepositoryResolved.GetType().Should().Be(typeof(VerifyUsersRepository));
            exchangeConditionRepositoryResolved.GetType().Should().Be(typeof(ExchangeConditionRepository));
            categoryRepositoryResolved.GetType().Should().Be(typeof(CategoryRepository));
            wishListRepositoryResolved.GetType().Should().Be(typeof(WishListRepository));
            subcriptionRepositoryResolved.GetType().Should().Be(typeof(SubcriptionRepository));
            subscriptionHistoryRepositoryResolved.GetType().Should().Be(typeof(SubscriptionHistoryRepository));
            ratingRepositoryResolved.GetType().Should().Be(typeof(RatingRepository));
            messageRepositoryResolved.GetType().Should().Be(typeof(MessageRepository));
            orderRepositoryResolved.GetType().Should().Be(typeof(OrderRepository));
            chatRoomRepositoryResolved.GetType().Should().Be(typeof(ChatRoomRepository));
            walletTransactionRepositoryResolved.GetType().Should().Be(typeof(WalletTransactionRepository));
            reportRepositoryResolved.GetType().Should().Be(typeof(ReportRepository));
        }
    }
}
