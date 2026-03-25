using System.ComponentModel.DataAnnotations.Schema;

namespace backend_app.Models
{
    public partial class Actionable
    { 
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}
