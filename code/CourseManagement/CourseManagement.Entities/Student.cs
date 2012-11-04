namespace CourseManagement.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Students")]
    public class Student : Person
    {
        public Student() : this(0, string.Empty, string.Empty)
        {
        }

        public Student(int id, string name, string email) : base(id, name, email)
        {
            this.Groups = new List<Group>();
            this.CreatedTickets = new List<Ticket>();
        }

        public virtual ICollection<Ticket> CreatedTickets { get; set; }

        public virtual ICollection<Group> Groups { get; set; }
    }
}
