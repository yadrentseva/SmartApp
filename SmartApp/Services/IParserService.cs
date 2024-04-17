using SmartApp.Models;

namespace SmartApp.Services
{
    public interface IParserService
    {
        Task<List<Comment>> GetCommentsAsync(string? profile);
        Task DownloadCommentsAsync();
    }
}
