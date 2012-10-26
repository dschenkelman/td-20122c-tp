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

        [ForeignKey("CourseCode")]
        public virtual Course Course { get; set; }

        public int CourseCode { get; set; }

        public virtual ICollection<Configuration> Configurations { get; set; }
    }
}
