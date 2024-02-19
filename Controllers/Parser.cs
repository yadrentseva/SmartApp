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

namespace SmartApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Parser : Controller
    {
        private HttpClient httpClient;
        private string connectionString;

        public Parser()
        {
            httpClient = new HttpClient();

            connectionString = "Host=localhost;Username=postgres;Password=12345678;Database=smart";
        }
        public IActionResult Index()
        {
            return View("");
        }

        [HttpGet("DownloadComments")]
        public async Task DownloadComments()
        {
            List<Comment> comments = await GetCommentsLastFivePages();

            if (comments.Count() > 0)
                SaveCommentsInBd(comments);
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

                                var blockTextComment = pageComment.Substring(startIndexTextComment + textCommentBlockStart.Length, endIndexTextComment - textCommentBlockStart.Length - startIndexTextComment);
                                var blockCommentId = pageComment.Substring(startIndexComment + commentBlockStart.Length, 8); // или до первой кавычки

                                if (Int32.TryParse(blockCommentId, out int commentId))
                                {
                                    var comment = new Comment()
                                    {
                                        CommentId = commentId,
                                        Message = blockTextComment
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

        private void SaveCommentsInBd(List<Comment> comments)
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var maxId = connection.Query("SELECT MAX(id) from comments").ToList();
            int currencyId = maxId[0].max != null ? maxId[0].max : 0;

            var newComments = comments.Where(c => c.CommentId > currencyId);
            if (newComments.Count() == 0) return;

            string sqlQuery = $@"INSERT INTO comments (id, message) VALUES (@{nameof(Comment.CommentId)}, @{nameof(Comment.Message)})";

            connection.Execute(sqlQuery, newComments);
        }

        [HttpGet("GetComments")]
        public List<Comment> GetComments()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            string selectQuery = $@"SELECT id as {nameof(Comment.CommentId)}, 
                                           message as {nameof(Comment.Message)} FROM comments";

            return connection.Query<Comment>(selectQuery).ToList();
        }

    }
}

