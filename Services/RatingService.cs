using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SmartApp.Models;
using System.Text.Json;

namespace SmartApp.Services
{
    public class RatingService: IRatingService
    {
        private readonly IOptions<SmartDBConnection> _smartDBConnectionAccessor;
        IDistributedCache cache;

        public RatingService(IDistributedCache distributedCache, IOptions<SmartDBConnection> smartDBConnectionAccessor)
        {
            _smartDBConnectionAccessor = smartDBConnectionAccessor;
            cache = distributedCache;
        }
        public async Task<Rating?> GetAuthorsRatingAsync(string profile)
        {
            Rating? rating = null;

            var ratingString = await cache.GetStringAsync(profile);

            if (ratingString != null) rating = JsonSerializer.Deserialize<Rating>(ratingString);

            if (rating == null)
            {
                using (SmartContext dbContext = new SmartContext(_smartDBConnectionAccessor))
                {
                    rating = await dbContext.rating.Where(r => r.AuthorProfile == profile).FirstOrDefaultAsync<Rating>(); 
                    if (rating != null)
                    {
                        ratingString = JsonSerializer.Serialize(rating);

                        await cache.SetStringAsync(rating.AuthorProfile, ratingString, new DistributedCacheEntryOptions { });
                    }
                }     
            }   
            return rating;
        }
    }
}
