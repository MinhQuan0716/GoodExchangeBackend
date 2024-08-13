using Application.InterfaceService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MobileAPI.MobileService;
using Application.Service;
using Microsoft.Extensions.DependencyInjection;
using Application.Util;
using Application.ApplicationCache;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
namespace MobileAPI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMobileAPIService(this IServiceCollection services,string secretKey,string cacheConnectionString) 
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IClaimService, ClaimService>();
            services.AddScoped<ICurrentUserIp, CurrentUserIp>();    
            services.AddScoped<ICurrentTime, CurrentTime>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWalletService, WalletService>();    
            services.AddScoped<IPaymentService, PaymentService>();  
            services.AddScoped<ISendMailHelper, SendMailHelper>();  
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUploadFile, UploadFile>();
            services.AddScoped<IExchangeConditionService, ExchangeConditionService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ISubcriptionService, SubcriptionService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IVerifyUserService, VerifyUserService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<ISubscriptionHistoryService, SubscriptionHistoryService>();
            services.AddScoped<IWalletTransactionService,WalletTransactionService>();
            /*            services.AddScoped<ISettingService, SettingService>();*/
            services.AddScoped<IPolicyService, PolicyService>();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(12);
                options.Cookie.Name = "GoodExchange";
                options.Cookie.IsEssential = true;
            });
            var options = ConfigurationOptions.Parse(cacheConnectionString); // host1:port1, host2:port2, ...
            options.Password = "MinhQuan@123";
            services.AddSingleton<IDatabase>(cfg =>
            {
                IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(options);
                return multiplexer.GetDatabase();
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,
                      ValidIssuer = secretKey,
                      ValidAudience = secretKey,
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                      ClockSkew = TimeSpan.FromSeconds(1)
                  };
              });
           
            return services;
        }
    }
}
