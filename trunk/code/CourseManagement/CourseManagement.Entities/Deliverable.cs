using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagement.Model
{
    public class Deliverable
    {
        public Deliverable()
        {
        }

        public Deliverable(DateTime receptionDate)
        {
            this.ReceptionDate = receptionDate;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime ReceptionDate { get; set; }

        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }

        public int GroupId { get; set; }

        public List<Attachment> Attachments { get; set; }
    }
}
