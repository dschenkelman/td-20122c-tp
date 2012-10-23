using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagement.Model
{
    public class Attachment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string FileName { get; set; }

        public string Location { get; set; }

        [ForeignKey("DeliverableId")]
        public virtual Deliverable Deliverable { get; set; }

        public int DeliverableId { get; set; }

        [ForeignKey("TicketId")]
        public virtual Ticket Ticket { get; set; }

        public virtual int TicketId { get; set; }
    }
}
