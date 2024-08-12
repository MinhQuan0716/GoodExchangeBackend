using Application.CacheEntity;
using Microsoft.Extensions.Hosting;

namespace MobileAPI.MobileBackgroundService
{
    public class SettingBackground :BackgroundService
    {
        private readonly Setting _setting;
        public SettingBackground(Setting setting)
        {
            _setting = setting;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
