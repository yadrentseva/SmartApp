using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SmartApp.Models;
using SmartApp.Services;

namespace SmartApp.Handlers
{
    public class LoadingCommentsLoggingHandler: IMyHandler
    {
        private readonly IOptions<SmartDBConnection> _smartDBConnectionAccessor;

        public LoadingCommentsLoggingHandler(IOptions<SmartDBConnection> smartDBConnectionAccessor)
        {
            _smartDBConnectionAccessor = smartDBConnectionAccessor;
        }
        
        public async Task Handle(LoadingCommentsRequest request)
        {
            var commentsService = new CommentsService(_smartDBConnectionAccessor);

            await commentsService.LogginLoadCommentsDBAsync(request);
        }
    }
}
