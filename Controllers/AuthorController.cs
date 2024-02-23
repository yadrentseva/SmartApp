using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using SmartApp.Models;
using System.Xml.Linq;

namespace SmartApp.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthorController : Controller
    {
        private readonly SmartConfig smartConfig;
        private readonly string connectionString;
        
        public AuthorController(SmartConfig _smartConfig)
        {
            smartConfig = _smartConfig;
            connectionString = smartConfig.ConnectionString; 
        }

        [HttpGet] 
        public IActionResult Index()
        {
            return Redirect("~/Author/GetAll");
        }

        [HttpGet] // [HttpPost]
        public async Task<Author?> Add([FromQuery] Author author)
        {
            var oldAuthor = await GetByProfile(new AuthorProfile() { Profile = author.Profile});
            if (oldAuthor != null) return null;

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            
            string commandText = $"INSERT INTO authors (profile, name) VALUES (@{nameof(Author.Profile)}, @{nameof(Author.Name)}) RETURNING profile, name";

            var results = await connection.QueryAsync<Author>(commandText, new { profile = author.Profile , name = author.Name });
            
            return (results.Count() != 0) ? results.FirstOrDefault() : null;
        }

        [HttpGet]
        public async Task<List<Author>> GetAll()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            string commandText = $@"SELECT profile as {nameof(Author.Profile)}, name as {nameof(Author.Name)} FROM authors";

            var results = await connection.QueryAsync<Author>(commandText);
            
            return results.ToList();
        }
        
        [HttpGet]
        public async Task<Author?> GetByProfile([FromQuery] AuthorProfile authorProfile)
        {
            var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            string commandText = $@"SELECT profile as {nameof(Author.Profile)}, name as {nameof(Author.Name)}
                                    FROM authors WHERE profile = @profile";

            var results = await connection.QueryAsync<Author>(commandText, new { profile = authorProfile.Profile });
            
            return (results.Count() != 0) ? results.FirstOrDefault() : null;
        }

        [HttpGet] // [HttpPut]
        public async Task<Author?> Update([FromQuery] AuthorProfile authorProfile, [FromQuery] Author author)
        {
            var oldAuthor = await GetByProfile(new AuthorProfile() { Profile = authorProfile.Profile });
            if (oldAuthor == null) return null;

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            string commandText = $"UPDATE authors SET name = @{nameof(Author.Name)} WHERE profile = @{nameof(Author.Profile)} RETURNING profile, name";

            var results = await connection.QueryAsync<Author>(commandText, new { profile = author.Profile, name = author.Name });

            return (results.Count() != 0) ? results.FirstOrDefault() : null;
        }

        [HttpGet] // [HttpDelete]
        public async Task Delete([FromQuery] AuthorProfile authorProfile)
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            string deleteQuery = $@"DELETE FROM authors WHERE profile = @profile";
            
            connection.Execute(deleteQuery, new { profile = authorProfile.Profile }); 
        }
    }
}
