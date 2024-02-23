using Microsoft.Extensions.FileSystemGlobbing;
using SmartApp.Controllers;
using System.Threading;

namespace SmartApp.Models
{
    public class LoadingService : IHostedService
    {
        ParserController parser;
        public LoadingService(ILogger<ParserController> _logger, SmartConfig _smartConfig)
        {
           parser = new ParserController(_logger, _smartConfig); // todo
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
