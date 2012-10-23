using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagement.Model
{
    [Table("Teachers")]
    public class Teacher : Person
    {
        public virtual ICollection<Ticket> AssignedTickets { get; set; }
    }
}
