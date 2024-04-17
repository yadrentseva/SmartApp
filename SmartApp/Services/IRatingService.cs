using SmartApp.Models;

namespace SmartApp.Services
{
    public interface IRatingService
    {
        Task<Rating?> GetAuthorsRatingAsync(string profile);
    }
}
