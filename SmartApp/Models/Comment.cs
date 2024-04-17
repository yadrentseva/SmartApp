using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartApp.Models
{
    public class Comment
    {
        [Key]
        [Column("id")]
        public int CommentId { get; set; }
        [Column("message")]
        public string Message { get; set; }
        [Column("authorprofile")]
        public Author Author { get; set; }    
    }
}
