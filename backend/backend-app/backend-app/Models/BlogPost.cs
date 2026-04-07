using backend_app.Enums;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_app.Models
{
    [Table("blog_posts")]
    public class BlogPost : Actionable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int AuthorId { get; set; }

        
        [Required, MaxLength(255)]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }


        [ForeignKey(nameof(AuthorId))]
        [JsonIgnore]
        public User Author { get; set; } = null!;

        [JsonIgnore]
        public ICollection<BlogComment> Comments { get; set; } = [];
    }
}