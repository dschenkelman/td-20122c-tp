namespace CourseManagement.Entities.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Reply
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int RelatedTicketId { get; set; }

        [ForeignKey("RelatedTicketId")]
        public virtual Ticket RelatedTicket { get; set; }

        public string EmailSubject { get; set; }

        public string EmailBody { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
