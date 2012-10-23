namespace CourseManagement.Entities.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Teachers")]
    public class Teacher : Person
    {
        public virtual ICollection<Ticket> AssignedTickets { get; set; }
    }
}
