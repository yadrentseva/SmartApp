using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartApp.Models
{
    public class Author
    {
        [Key]
        [Column("profile")]
        public string Profile { get; set; }
        [Column("name")]
        public string? Name { get; set; }
        public Rating? Rating { get; set; }
    }
}
