namespace CourseManagement.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
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
        [DisplayName("Subject")]
        public string MessageSubject { get; set; }

        [DisplayName("Body")]
        public string MessageBody { get; set; }

        [Required]
        [DisplayName("Private?")]
        public bool IsPrivate { get; set; }

        [Required]
        [DisplayName("Creation Date")]
        public DateTime DateCreated { get; set; }

        [Required]
        [DisplayName("Last Updated On")]
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
