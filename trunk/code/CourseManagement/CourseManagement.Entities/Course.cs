using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagement.Model
{
    public class Course
    {

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

        public int Year { get; set; }

        public int Semester { get; set; }
        
        public int SubjectId { get; set; }

        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; }

        public virtual ICollection<Student> Students { get; set; }

        public virtual ICollection<Teacher> Teachers { get; set; }
    }
}