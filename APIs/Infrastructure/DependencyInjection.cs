using Application;
using Application.InterfaceRepository;
using Application.InterfaceService;
using Infrastructure.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services,string databaseConnectionString)
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(databaseConnectionString).EnableSensitiveDataLogging());
            services.AddTransient<IDbConnection>((sp) => new SqlConnection(databaseConnectionString));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IVerifyUsersRepository, VerifyUsersRepository>();
            services.AddScoped<IExchangeConditionRepository, ExchangeConditionRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IWishListRepository, WishListRepository>();  
            services.AddScoped<ISubcriptionRepository,SubcriptionRepository>();
            services.AddScoped<ISubscriptionHistoryRepository, SubscriptionHistoryRepository>();
            services.AddScoped<IRatingRepository, RatingRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IRequestRepository, RequestRepository>();
            services.AddScoped<IChatRoomRepository, ChatRoomRepository>();
            return services;    
        }
    }
}
