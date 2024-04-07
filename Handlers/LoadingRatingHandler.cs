using Microsoft.Extensions.Options;
using SmartApp.Models;
using SmartApp.Services;

namespace SmartApp.Handlers
{
    public class LoadingRatingHandler : IMyHandler
    {
        private readonly IOptions<SmartDBConnection> _smartDBConnectionAccessor;
        
        public LoadingRatingHandler(IOptions<SmartDBConnection> smartDBConnectionAccessor)
        {
            _smartDBConnectionAccessor = smartDBConnectionAccessor;
        }
        public async Task Handle(string body)
        {
            var updateRatingService = new UpdateAuthorRatingService(_smartDBConnectionAccessor, new SmartlabData());

            await updateRatingService.LoadAuthorRatingFromSmartlabAsync(); 
        }
    }
}
