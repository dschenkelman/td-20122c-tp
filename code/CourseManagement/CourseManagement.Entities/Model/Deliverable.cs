namespace CourseManagement.Entities.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Deliverable
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime ReceptionDate { get; set; }

        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }

        public int GroupId { get; set; }
    }
}
