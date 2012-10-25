using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagement.Model
{
    public class Account
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        [ForeignKey("SubjectCode")]
        public virtual Subject Subject { get; set; }

        public int SubjectCode { get; set; }

        public virtual ICollection<Configuration> Courses { get; set; }
    }
}
