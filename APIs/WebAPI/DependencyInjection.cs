using Application.CacheService;
using Application.InterfaceService;
using Application.Service;
using Application.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using WebAPI.WebService;

namespace WebAPI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebAPIService(this IServiceCollection services, string secretKey,string cacheConnectionString)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IClaimService,ClaimService>();
            services.AddScoped<ICurrentTime, CurrentTime>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ISendMailHelper,SendMailHelper>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUploadFile, UploadFile>();
            services.AddScoped<ISubcriptionService, SubcriptionService>();
            services.AddDistributedMemoryCache();
            services.AddSession();
            var options = ConfigurationOptions.Parse(cacheConnectionString); // host1:port1, host2:port2, ...
            options.Password = "MinhQuan@123";
            services.AddScoped<IDatabase>(cfg =>
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
