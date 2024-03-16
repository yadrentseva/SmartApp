using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SmartApp.Models;
using SmartApp.Services;

namespace SmartApp.Handlers
{
    public class LogginLoadCommentsDBHandler: IMyHandler
    {
        private readonly IOptions<SmartDBConnection> _smartDBConnectionAccessor;

        public LogginLoadCommentsDBHandler(IOptions<SmartDBConnection> smartDBConnectionAccessor)
        {
            _smartDBConnectionAccessor = smartDBConnectionAccessor;
        }
        public async Task Handle(string body)
        {
            var commentsService = new CommentsService(_smartDBConnectionAccessor);

            var info = JsonConvert.DeserializeObject<InfoLoadingService>(body);

            await commentsService.LogginLoadCommentsDBAsync(info);
        }
    }
}
