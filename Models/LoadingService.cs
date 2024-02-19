using Microsoft.Extensions.FileSystemGlobbing;
using SmartApp.Controllers;

namespace SmartApp.Models
{
    public class LoadingService : IHostedService
    {
        Parser parser;
        public LoadingService()
        {
            parser = new Parser();
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await DownloadCommentsAsync();
                }
                catch (Exception ex)
                {
                    // обработка ошибки однократного неуспешного выполнения фоновой задачи
                }
                await Task.Delay(600000, cancellationToken);
            }
        }
        private async Task DownloadCommentsAsync()
        {
            await parser.DownloadComments();
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            parser?.Dispose();
            await Task.CompletedTask;
        }
    }
}
