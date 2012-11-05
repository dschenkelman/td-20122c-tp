namespace CourseManagement.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Account
    {
        public Account()
        {
            this.Configurations = new List<Configuration>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public virtual ICollection<Configuration> Configurations { get; set; }

        public virtual Course Course { get; set; }
    }
}
