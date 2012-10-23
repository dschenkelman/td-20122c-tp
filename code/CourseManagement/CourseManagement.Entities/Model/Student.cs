namespace CourseManagement.Entities.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Students")]
    public class Student : Person
    {
        public virtual ICollection<Ticket> CreatedTickets { get; set; }

        public virtual ICollection<Group> Groups { get; set; }
    }
}
