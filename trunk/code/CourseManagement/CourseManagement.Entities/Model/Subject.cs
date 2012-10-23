using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Entities.Model
{
    using System.Collections.Generic;

    public class Subject
    {
        [Key]
        public int Code { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }
}
