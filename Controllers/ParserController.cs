using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net;
using System.Xml;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Linq;
using SmartApp.Models;
using Dapper;
using Npgsql;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using NpgsqlTypes;
using static Dapper.SqlMapper;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace SmartApp.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ParserController : Controller
    {
        private HttpClient httpClient;
        private ILogger<ParserController> logger;
        private readonly SmartConfig smartConfig;
        private readonly string connectionString;
        public ParserController(ILogger<ParserController> _logger, SmartConfig _smartConfig)
        {
            httpClient = new HttpClient();
            logger = _logger;
            smartConfig = _smartConfig;
            connectionString = smartConfig.ConnectionString; 
        }
       
        public IActionResult Index()
        {
            return View("");
        }

        [HttpGet]
        public async Task DownloadComments()
        {
            logger.LogInformation($"Path: /DownloadComments Start time: {DateTime.Now.ToLongTimeString()}");
            
            List<Comment> comments = await GetCommentsLastFivePages();
           
            if (comments.Count() > 0)
                SaveCommentsInBd(comments);

            logger.LogInformation($"Path: /DownloadComments End time: {DateTime.Now.ToLongTimeString()}");
        }

        [HttpGet("{profile?}")]
        public List<Comment> GetComments(string? profile) // todo
        {
            logger.LogInformation($"Path: /GetComments/{profile} Start time: {DateTime.Now.ToLongTimeString()}");

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            string selectQuery = $@"SELECT comments.id as {nameof(Comment.CommentId)}, 
                                           comments.message as {nameof(Comment.Message)},
                                           authors.profile as {nameof(Author.Profile)},
                                           authors.name as {nameof(Author.Name)} FROM comments
                                           LEFT JOIN authors ON comments.authorprofile = authors.profile
                                           WHERE authorprofile = @profile OR @profile is null";
            var results = connection.Query<Comment, Author, Comment>(selectQuery,
               ((Comment, Author) =>
               {
                   Comment.Author = Author;
                   return Comment;
               }),
               new { profile = profile },
               splitOn: "Profile").ToList();

            return results;  
        }

        private async Task<List<Comment>> GetCommentsLastFivePages()
        {
            List<Comment> comments = new();

            string commentBlock = "<li class=\"comments-total\">";
            string hrefBlockStart = "<a href=\"/blog/";
            string hrefBlockEnd = ".php#comments";
            string countBlockStart = "<span class=\"red\">";
            string countBlockEnd = "</span>";


            for (int i = 5; i > 0; i--)
            {
                var pageTopic = await httpClient.GetStringAsync($"https://smart-lab.ru/index/page{i}/");

                int startIndex;
                int endIndex = 0;

                while (true)
                {
                    startIndex = pageTopic.IndexOf(commentBlock, endIndex);
                    endIndex = startIndex + 1;

                    if (startIndex == -1) break;

                    int startIndexCount = pageTopic.IndexOf(countBlockStart, startIndex);
                    int endIndexCount = pageTopic.IndexOf(countBlockEnd, startIndexCount);

                    var blockCount = pageTopic.Substring(startIndexCount + countBlockStart.Length, endIndexCount - countBlockStart.Length - startIndexCount);

                    if (Int32.TryParse(blockCount, out int countComments))
                    {
                        if (countComments == 0) continue;

                        int startIndexHref = pageTopic.IndexOf(hrefBlockStart, startIndex);
                        int endIndexHref = pageTopic.IndexOf(hrefBlockEnd, startIndexHref);
                        if (startIndexHref == -1 || endIndexHref == -1) continue;

                        var blockHref = pageTopic.Substring(startIndexHref + hrefBlockStart.Length, endIndexHref - hrefBlockStart.Length - startIndexHref);

                        if (Int32.TryParse(blockHref, out int topicId))
                        {
                            string commentBlockStart = "<div id=\"comment_content_id_";
                            string textCommentBlockStart = "<div class=\"text\">";
                            string textCommentBlockEnd = "</div>";
                            string profileBlockStart = "<div class=\"author\"><a href=\"/profile/";
                            string traderBlockStart = "/\" class=\"trader_other\">";
                            string authorBlockEnd = "</a>";

                            var pageComment = await httpClient.GetStringAsync($"https://smart-lab.ru/blog/{topicId}.php#comments");

                            int startIndexComment;
                            int endIndexComment = 0;
                            while (true)
                            {
                                startIndexComment = pageComment.IndexOf(commentBlockStart, endIndexComment);
                                endIndexComment = startIndexComment + 1;

                                if (startIndexComment == -1) break;

                                int startIndexTextComment = pageComment.IndexOf(textCommentBlockStart, startIndexComment);
                                int endIndexTextComment = pageComment.IndexOf(textCommentBlockEnd, startIndexTextComment);
                                int startIndexProfileComment = pageComment.IndexOf(profileBlockStart, startIndexComment);
                                int startIndexTraderComment = pageComment.IndexOf(traderBlockStart, startIndexComment);
                                int endIndexProfileComment = pageComment.IndexOf(authorBlockEnd, startIndexTraderComment);

                                var blockTextComment = pageComment.Substring(startIndexTextComment + textCommentBlockStart.Length, endIndexTextComment - textCommentBlockStart.Length - startIndexTextComment);
                                var blockCommentId = pageComment.Substring(startIndexComment + commentBlockStart.Length, 8); // или до первой кавычки
                                var authorProfile = pageComment.Substring(startIndexProfileComment + profileBlockStart.Length, startIndexTraderComment - profileBlockStart.Length - startIndexProfileComment);
                                var authorName = pageComment.Substring(startIndexTraderComment + traderBlockStart.Length, endIndexProfileComment - traderBlockStart.Length - startIndexTraderComment);

                                if (Int32.TryParse(blockCommentId, out int commentId))
                                {
                                    var comment = new Comment()
                                    {
                                        CommentId = commentId,
                                        Message = blockTextComment,
                                        Author = new Author() { Name = authorName, Profile = authorProfile }

                                    };
                                    comments.Add(comment);
                                }
                            }
                        }
                    }
                }
            }
            return comments;
        }

        private async Task SaveCommentsInBd(List<Comment> comments)
        {
            logger.LogDebug("Saving comments to database...");
            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                connection.Open();

                var maxId = Convert.ToInt32(connection.ExecuteScalar("SELECT MAX(id) from comments"));

                string blackListQuery = $"SELECT authorprofile as {nameof(Author.Profile)} from blacklist";
                var blackList = await connection.QueryAsync<String>(blackListQuery);

                var newComments = comments.Where(c => c.CommentId > maxId && !blackList.Contains(c.Author.Profile)).ToList();
                if (newComments.Count() == 0)
                {
                    logger.LogDebug("No new comments to upload.");
                    return;
                }

                var allAuthors = (from nCom in newComments select new { Profile = nCom.Author.Profile, Name = nCom.Author.Name }).Distinct();

                string sqlQueryAuthors = $@"INSERT INTO authors(profile, name)
                                            VALUES (@Profile, @Name)
                                            ON CONFLICT (profile) DO UPDATE SET name = @Name";
                foreach (var author in allAuthors)
                {
                    await connection.ExecuteAsync(sqlQueryAuthors, new { Profile = author.Profile, Name = author.Name });
                }
                logger.LogDebug("Authors loaded successfully.");

                string sqlQueryComments = $@"INSERT INTO comments(id, message, authorProfile)
                                            VALUES(@id, @message, @authorProfile)";
                foreach (var comment in newComments)
                {
                    await connection.ExecuteAsync(sqlQueryComments, new { id = comment.CommentId, message = comment.Message, authorProfile = comment.Author.Profile });
                }
                logger.LogDebug("Comments loaded successfully."); 
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to load comments into source data for reason {ex}");
            }
        }
    }
}

