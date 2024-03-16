using Microsoft.AspNetCore.Mvc;
using SmartApp.Models;

namespace SmartApp.Services
{
    public interface IBlackListService
    {
        Task<List<BlackList>> GetAllAsync();
        Task<Boolean> InBlackList(string profile); 
        Task AddAsync(AuthorsModel authorsModel);
        Task DeleteAsync(string profile); 
    }
}
