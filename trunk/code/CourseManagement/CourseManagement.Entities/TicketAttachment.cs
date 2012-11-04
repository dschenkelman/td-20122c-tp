namespace CourseManagement.Model
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TicketAttachments")]
    public class TicketAttachment : Attachment
    {
        [ForeignKey("TicketId")]
        public virtual Ticket Ticket { get; set; }

        public virtual int TicketId { get; set; }
    }
}
