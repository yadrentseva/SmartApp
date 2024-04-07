using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SmartApp.Services;

namespace SmartApp.Models
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly SmartContext _dbContext;

        public AuthorRepository(IOptions<SmartDBConnection> smartDBConnectionAccessor)
        {

            _dbContext = new SmartContext(smartDBConnectionAccessor); 
        }

        public async Task<Author> AddAuthorAsync(Author author)
        {
            var result = _dbContext.authors.Add(author);
            await _dbContext.SaveChangesAsync(); 
            return result.Entity;
        }

        public async Task<int> DeleteAuthorAsync(string profile)
        {
            var filteredData = _dbContext.authors.Where(a => a.Profile == profile).FirstOrDefault();
            if (filteredData != null)
            {
                _dbContext.authors.Remove(filteredData);
                return await _dbContext.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<Author> GetAuthorByProfileAsync(string profile)
        {
            return await _dbContext.authors.Where(a => a.Profile == profile).FirstOrDefaultAsync();
        }

        public async Task<List<Author>> GetAuthorsListAsync()
        {
            return await _dbContext.authors.ToListAsync<Author>();
        }

        public async Task<int> UpdateAuthorAsync(Author author)
        {
            _dbContext.authors.Update(author);
            return await _dbContext.SaveChangesAsync();
        }
    }
}
