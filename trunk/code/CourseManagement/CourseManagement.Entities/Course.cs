namespace CourseManagement.Model
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Course
    {
        public Course()
        {
            this.Students = new List<Student>();
            this.Teachers = new List<Teacher>();
        }

        public Course(int semester, int year, int subjectId)
        {
            this.Semester = semester;
            this.Year = year;
            this.SubjectId = subjectId;
            this.Students = new List<Student>();
            this.Teachers = new List<Teacher>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Range(2012, int.MaxValue)]
        public int Year { get; set; }

        [Range(1, 2)]
        public int Semester { get; set; }
        
        public int SubjectId { get; set; }

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }

        public virtual ICollection<Student> Students { get; set; }

        public virtual ICollection<Teacher> Teachers { get; set; }

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        public int AccountId { get; set; }

        [DisplayName("Public DL")]
        public string PublicDistributionList { get; set; }
    }
}