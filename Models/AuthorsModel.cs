using System.ComponentModel.DataAnnotations;

namespace SmartApp.Models
{
    public class AuthorsModel
    {
        [Required]
        public string Profile { get; set; }
        
        [Required]
        public string Name { get; set; }
    }
}
