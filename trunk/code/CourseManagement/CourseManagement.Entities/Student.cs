using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagement.Model
{
    [Table("Students")]
    public class Student : Person
    {
        public virtual ICollection<Ticket> CreatedTickets { get; set; }

        public virtual ICollection<Group> Groups { get; set; }
    }
}
