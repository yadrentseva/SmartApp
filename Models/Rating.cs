using System.ComponentModel.DataAnnotations;

namespace SmartApp.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }
        public int Place { get; set; }
        public int Forum { get; set; }
        public int Blog30Days { get; set; }
        public int OverallAllTime { get; set; }
        public int CountReading { get; set; }

        public string AuthorProfile { get; set; }
        public Author Author { get; set;}
    }    
}
