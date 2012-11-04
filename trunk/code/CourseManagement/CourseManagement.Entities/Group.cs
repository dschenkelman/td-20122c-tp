namespace CourseManagement.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Group
    {
        public Group()
        {
            this.Students = new List<Student>();
            this.Deliverables = new List<Deliverable>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        public virtual ICollection<Student> Students { get; set; }

        public virtual ICollection<Deliverable> Deliverables { get; set; }

    }
}
