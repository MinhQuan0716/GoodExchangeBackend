using Application.InterfaceService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CacheEntity
{
    public class Setting
    {
        private readonly IServiceProvider _serviceProvider;

        public Setting(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            using (var scope = _serviceProvider.CreateScope())
            {
                var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                cacheService.SetData<float>("POST_PRICE", 15000, null);
                cacheService.SetData<int>("CANCELLED_AMOUNT", 2, null);
            }
        }

        public bool UpdatePostPrice(float price)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                    cacheService.UpdateData("POST_PRICE", price);
                }
                return true; // Return true if the operation succeeds
            }
            catch (Exception ex)
            {
                // Log the exception if necessary (logging not shown here)
                return false; // Return false if an error occurs
            }
        }

        public bool UpdateCancelledAmount(int amount)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                    cacheService.UpdateData("CANCELLED_AMOUNT", amount);
                }
                return true; // Return true if the operation succeeds
            }
            catch (Exception ex)
            {
                // Log the exception if necessary (logging not shown here)
                return false; // Return false if an error occurs
            }
        }
        public float GetPostPrice()
        {
            
            using(var scope = _serviceProvider.CreateScope())
            {
                var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                return cacheService.GetData<float>("POST_PRICE");
            }
        }
        public int GetOrderCancellAmount()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                return cacheService.GetData<int>("CANCELLED_AMOUNT");
            }
        }
    }
}
