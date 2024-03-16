using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using SmartApp.Models;

namespace SmartApp.Services
{
    public class CommentsService
    {
        string _connectionString;

        public CommentsService(IOptions<SmartDBConnection> smartDBConnectionAccessor)
        {
            _connectionString = smartDBConnectionAccessor.Value.ConnectionString;
        }

        public async Task LoadCommentsFromSmartlabAsync()
        {
            List<Comment> comments = await GetCommentsLastFivePages();

            if (comments.Count() > 0)
                await SaveCommentsInDB(comments);
        }

        public async Task LogginLoadCommentsDBAsync(InfoLoadingService info)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sqlQueryComments = $@"INSERT INTO logcomments(timerequest, username)
                                            VALUES(@timerequest, @username)";
            await connection.ExecuteAsync(sqlQueryComments, new { timerequest = info.TimeRequest, username = info.User});
        }


        private async Task<List<Comment>> GetCommentsLastFivePages()
        {
            var httpClient = new HttpClient();

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

                    if (int.TryParse(blockCount, out int countComments))
                    {
                        if (countComments == 0) continue;

                        int startIndexHref = pageTopic.IndexOf(hrefBlockStart, startIndex);
                        int endIndexHref = pageTopic.IndexOf(hrefBlockEnd, startIndexHref);
                        if (startIndexHref == -1 || endIndexHref == -1) continue;

                        var blockHref = pageTopic.Substring(startIndexHref + hrefBlockStart.Length, endIndexHref - hrefBlockStart.Length - startIndexHref);

                        if (int.TryParse(blockHref, out int topicId))
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

                                if (int.TryParse(blockCommentId, out int commentId))
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

        private async Task SaveCommentsInDB(List<Comment> comments)
        {

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                var maxId = Convert.ToInt32(connection.ExecuteScalar("SELECT MAX(id) from comments"));

                string blackListQuery = $"SELECT authorprofile as {nameof(Author.Profile)} from blacklist";
                var blackList = await connection.QueryAsync<string>(blackListQuery);

                var newComments = comments.Where(c => c.CommentId > maxId && !blackList.Contains(c.Author.Profile)).ToList();
                if (newComments.Count() == 0)
                {

                    return;
                }

                var allAuthors = (from nCom in newComments select new { nCom.Author.Profile, nCom.Author.Name }).Distinct();

                string sqlQueryAuthors = $@"INSERT INTO authors(profile, name)
                                            VALUES (@Profile, @Name)
                                            ON CONFLICT (profile) DO UPDATE SET name = @Name";
                foreach (var author in allAuthors)
                {
                    await connection.ExecuteAsync(sqlQueryAuthors, new { author.Profile, author.Name });
                }


                string sqlQueryComments = $@"INSERT INTO comments(id, message, authorProfile)
                                            VALUES(@id, @message, @authorProfile)";
                foreach (var comment in newComments)
                {
                    await connection.ExecuteAsync(sqlQueryComments, new { id = comment.CommentId, message = comment.Message, authorProfile = comment.Author.Profile });
                }

            }
            catch (Exception ex)
            {
                // _logger.LogError($"Failed to load comments into source data for reason {ex}");
            }
        }

    }
}
