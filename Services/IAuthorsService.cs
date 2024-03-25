using Microsoft.AspNetCore.Mvc;
using SmartApp.Models;

namespace SmartApp.Services
{
    public interface IAuthorsService
    {
        Task<List<Author>> GetAllAsync();
        Task<Author?> GetByProfileAsync(string profile); 
        Task<Author?> CreateAsync(AuthorsModel authorsModel);
        Task<Author?> UpdateAsync(AuthorsModel authorsModel);
        Task DeleteAsync(string profile);
    }
}
