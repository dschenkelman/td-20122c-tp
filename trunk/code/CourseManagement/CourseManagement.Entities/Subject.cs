using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CourseManagement.Model
{
    public class Subject
    {
        [Key]
        public int Code { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }
}
