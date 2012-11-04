using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagement.Model
{
    [Table("Teachers")]
    public class Teacher : Person
    {
        public Teacher() : base()
        {
        }

        public Teacher(int id, string name, string email) : base(id, name, email)
        {
        }

        public virtual ICollection<Ticket> AssignedTickets { get; set; }
    }
}
