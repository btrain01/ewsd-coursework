using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_app.Models
{
    [Table("blog_comments")]
    public class BlogComment : Actionable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PostId { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [Required]
        public string Body { get; set; } = string.Empty;
        
        [ForeignKey(nameof(PostId))]
        [JsonIgnore]
        public BlogPost Post { get; set; }

        [ForeignKey(nameof(AuthorId))]
        [JsonIgnore]
        public User Author { get; set; }

    }
}