using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using SmartApp.Models;

namespace SmartApp.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class BlackListController : Controller
    {
        private readonly SmartConfig smartConfig;
        private readonly string connectionString;
        public BlackListController(SmartConfig _smartConfig)
        {
            smartConfig = _smartConfig;
            connectionString = smartConfig.ConnectionString;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Redirect("~/BlackList/GetAll");
        }

        [HttpGet] // [HttpPost]
        public async Task Add([FromQuery] AuthorProfile authorProfile)
        {
            if (await AuthorCreateNotAddBlackList(authorProfile.Profile) == false) return;

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            string createQuery = $@"INSERT INTO blacklist(authorprofile) VALUES (@profile)";

            await connection.ExecuteAsync(createQuery, new { profile = authorProfile.Profile });
        }

        [HttpGet] // [HttpDelete]
        public async Task Delete([FromQuery] AuthorProfile authorProfile)
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            string deleteQuery = $@"DELETE FROM blacklist WHERE authorprofile = @profile";
            
            await connection.ExecuteAsync(deleteQuery, new { profile = authorProfile.Profile });
        }

        [HttpGet]
        public async Task<List<Author>> GetAll()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            string selectQuery = $@"SELECT authors.profile as {nameof(Author.Profile)}, 
                                           authors.name as {nameof(Author.Name)} FROM authors  
                                           INNER JOIN blacklist ON authors.profile = blacklist.authorprofile 
                                           ORDER BY authors.profile";

            var results = await connection.QueryAsync<Author>(selectQuery);

            return results.ToList(); 
        }
       
        private async Task<bool> AuthorCreateNotAddBlackList(string profile)
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            string selectQuery = $@"SELECT authors.profile as {nameof(Author.Profile)}, 
                                           authors.name as {nameof(Author.Name)} FROM authors  
                                           LEFT JOIN blacklist ON authors.profile = blacklist.authorprofile 
                                           WHERE authors.profile = @profile AND blacklist.authorprofile is Null";

            var authors = await connection.QueryAsync<Author>(selectQuery, new { profile = profile });
            return authors.Count() > 0;
        }
    }
}
