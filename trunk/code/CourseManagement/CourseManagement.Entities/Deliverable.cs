namespace CourseManagement.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Deliverable
    {
        public Deliverable() : this(default(DateTime))
        {
        }

        public Deliverable(DateTime receptionDate)
        {
            this.ReceptionDate = receptionDate;
            this.Attachments = new List<DeliverableAttachment>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime ReceptionDate { get; set; }

        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }

        public int GroupId { get; set; }

        public List<DeliverableAttachment> Attachments { get; set; }
    }
}
