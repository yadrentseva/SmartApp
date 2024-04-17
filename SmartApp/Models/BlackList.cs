using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartApp.Models
{
    public class BlackList
    {
        [Key]
        [Column("authorprofile")]
        public string Authorprofile {get; set;}
    }
}
