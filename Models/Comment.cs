namespace SmartApp.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Message { get; set; }
        public Author Author { get; set; }
        
    }
}
