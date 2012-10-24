using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagement.Model
{
    public abstract class Person
    {
        protected Person(int id, string name, string email)
        {
            this.Id = id;
            this.Name = name;
            this.EmailAdress = email;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string EmailAdress { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }
}