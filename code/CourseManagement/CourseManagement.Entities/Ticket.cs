namespace CourseManagement.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Ticket
    {
        public Ticket()
        {
            this.Attachments = new List<TicketAttachment>();
            this.Replies = new List<Reply>();
        }

        public virtual ICollection<TicketAttachment> Attachments { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string MessageSubject { get; set; }
        
        [Required]
        public string MessageBody { get; set; }

        [Required]
        public bool IsPrivate { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }

        [Required]
        public TicketState State { get; set; }

        public int? TeacherId { get; set; }

        public int StudentId { get; set; }
        
        [ForeignKey("TeacherId")]
        public virtual Teacher AssignedTeacher { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Creator { get; set; }

        public virtual ICollection<Reply> Replies { get; set; }
    }
}
