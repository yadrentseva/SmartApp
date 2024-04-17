namespace SmartApp.Models
{
    public interface IAuthorRepository
    {
        public Task<List<Author>> GetAuthorsListAsync();
        public Task<Author> GetAuthorByProfileAsync(string profile);
        public Task<Author> AddAuthorAsync(Author author);
        public Task<int> UpdateAuthorAsync(Author author);
        public Task<int> DeleteAuthorAsync(string profile);
    }
}
