using Microsoft.Extensions.Options;
using SmartApp.Models;
using SmartApp.Services;

namespace SmartApp.Handlers
{
    public class LoadingCommentsHandler: IMyHandler
    {
        private readonly IOptions<SmartDBConnection> _smartDBConnectionAccessor;

        public LoadingCommentsHandler(IOptions<SmartDBConnection> smartDBConnectionAccessor)
        {
            _smartDBConnectionAccessor = smartDBConnectionAccessor;
        }

        public async Task Handle(string body)
        {
            var commentsService = new CommentsService(_smartDBConnectionAccessor);

            await commentsService.LoadCommentsFromSmartlabAsync();
        }
        
    }
}
