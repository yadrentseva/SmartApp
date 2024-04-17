namespace SmartApp.Models
{
    public interface IDataManager
    {
        Task<List<string>> GetRatingAuthorsAsync();
    }
}
