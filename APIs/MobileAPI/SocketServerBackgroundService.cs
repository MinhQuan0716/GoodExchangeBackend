
using Application.InterfaceService;
using Application.Service;

namespace MobileAPI
{
    public class SocketServerBackgroundService : BackgroundService
    {
       // private readonly ISocketServerService _socketServerService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SocketServerBackgroundService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory=serviceScopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using(var scope=_serviceScopeFactory.CreateScope())
            {
              
                try
                {
                    var _socketServerService = scope.ServiceProvider.GetRequiredService<ISocketServerService>();
                    _socketServerService.Start();
                    // Optionally log a message indicating successful start
                    Console.WriteLine("Socket server started successfully.");

                    // Keep the service running
                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
                catch (Exception ex)
                {
                    // Log any exceptions that occurred during server start
                    Console.WriteLine($"Socket server failed to start: {ex.Message}");
                }
            }
           
        }
    }
}
