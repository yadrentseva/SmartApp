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
                var commentsRequest = new LoadingCommentsRequest() { TimeRequest = DateTime.Now, User = "veronika" };
                _mqService.SendMessage("LoadingComments", commentsRequest);

                var ratingRequest = new LoadingRatingRequest() { Count = 10}; 
                _mqService.SendMessage("LoadingRating", ratingRequest);

                await Task.Delay(100000, cancellationToken);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }

}
