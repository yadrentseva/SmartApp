using SmartApp.Models;
using SmartApp.RabbitMQ;

namespace SmartApp.Services
{
    public class LoadingService : IHostedService
    {
        private readonly IRabbitMqService _mqService;

        public LoadingService(IRabbitMqService mqService)
        {
            _mqService = mqService;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                RunLoad(cancellationToken);
            });
        }

        async Task RunLoad(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var info = new InfoLoadingService() { TimeRequest = DateTime.Now, User = "veronika" };
                _mqService.SendMessage("DataUpdateRequest", info);

                await Task.Delay(100000, cancellationToken);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }

}
