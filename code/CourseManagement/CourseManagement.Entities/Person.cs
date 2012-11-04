namespace CourseManagement.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public abstract class Person
    {
        public Person()
        {
        }

        protected Person(int id, string name, string messagingSystemId)
        {
            this.Id = id;
            this.Name = name;
            this.MessagingSystemId = messagingSystemId;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string MessagingSystemId { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }
}