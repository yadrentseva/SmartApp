using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using SmartApp.Models;

namespace SmartApp.Services
{
    public class AuthorsService: IAuthorsService
    {
        private readonly string _connectionString;

        public AuthorsService(IOptions<SmartDBConnection> smartDBConnectionAccessor)
        {
            _connectionString = smartDBConnectionAccessor.Value.ConnectionString;
        }

        public async Task<Author?> CreateAsync(AuthorsModel authorsModel)
        {
            var oldAuthor = await AuthorByProfile(authorsModel.Profile);
            if (oldAuthor != null) return null;

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string commandText = $"INSERT INTO authors (profile, name) VALUES (@{nameof(Author.Profile)}, @{nameof(Author.Name)}) RETURNING profile, name";

            var results = await connection.QueryAsync<Author>(commandText, new { profile = authorsModel.Profile, name = authorsModel.Name });

            return (results.Count() != 0) ? results.FirstOrDefault() : null;
        }

        public async Task DeleteAsync(string profile)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string deleteQuery = $@"DELETE FROM authors WHERE profile = @profile";

            await connection.ExecuteAsync(deleteQuery, new { profile = profile });
        }

        public async Task<List<Author>> GetAllAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string commandText = $@"SELECT profile as {nameof(Author.Profile)}, name as {nameof(Author.Name)} FROM authors";

            var results = await connection.QueryAsync<Author>(commandText);

            return results.ToList();
        }

        public async Task<Author?> GetByProfileAsync(string profile) 
        {
            return await AuthorByProfile(profile);
        }

        public async Task<Author?> UpdateAsync(AuthorsModel authorsModel)
        {
            var oldAuthor = await AuthorByProfile(authorsModel.Profile);
            if (oldAuthor == null) return null;

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string commandText = $"UPDATE authors SET name = @{nameof(Author.Name)} WHERE profile = @{nameof(Author.Profile)} RETURNING profile, name";

            var results = await connection.QueryAsync<Author>(commandText, new { profile = authorsModel.Profile, name = authorsModel.Name });

            return (results.Count() != 0) ? results.FirstOrDefault() : null;
        }


        private async Task<Author?> AuthorByProfile(string profile)
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string commandText = $@"SELECT profile as {nameof(Author.Profile)}, name as {nameof(Author.Name)}
                                    FROM authors WHERE profile = @profile";

            var results = await connection.QueryAsync<Author>(commandText, new { profile = profile });

            return (results.Count() != 0) ? results.FirstOrDefault() : null;
        }
    }
}
