using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using SmartApp.Models;

namespace SmartApp.Services
{
    public class ParserService : IParserService
    {
        private readonly ILogger<ParserService> _logger;
        private readonly string _connectionString;

        public ParserService(ILogger<ParserService> logger, IOptions<SmartDBConnection> smartDBConnectionAccessor)
        {
            _logger = logger;
            _connectionString = smartDBConnectionAccessor.Value.ConnectionString;

        }

        public async Task<List<Comment>> GetCommentsAsync(string? profile)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            string selectQuery = $@"SELECT comments.id as {nameof(Comment.CommentId)}, 
                                           comments.message as {nameof(Comment.Message)},
                                           authors.profile as {nameof(Author.Profile)},
                                           authors.name as {nameof(Author.Name)} FROM comments
                                           LEFT JOIN authors ON comments.authorprofile = authors.profile
                                           WHERE authorprofile = @profile OR @profile is null";
            var results = await connection.QueryAsync<Comment, Author, Comment>(selectQuery,
               ((Comment, Author) =>
               {
                   Comment.Author = Author;
                   return Comment;
               }),
               new { profile = profile },
               splitOn: "Profile");

            return results.ToList();
        }

        public async Task DownloadCommentsAsync()
        {
            
        }
    }
}
