using System.ComponentModel.DataAnnotations.Schema;

namespace backend_app.Models
{
    public partial class Actionable
    {
        public int Active { get; set; }
        public string createdBy { get; set; }
        public string modifiedBy { get; set; }
        
        private DateTime _dateCreated;
        private DateTime _dateModified;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated { get => _dateCreated; set => _dateCreated = value; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateModified { get => _dateModified; set => _dateModified = value; }
    }
}
