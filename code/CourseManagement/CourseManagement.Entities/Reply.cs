namespace CourseManagement.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Reply
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RelatedTicketId { get; set; }

        [ForeignKey("RelatedTicketId")]
        public virtual Ticket RelatedTicket { get; set; }

        public string MessageSubject { get; set; }

        public string MessageBody { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
