namespace CourseManagement.Model
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("DeliverableAttachments")]
    public class DeliverableAttachment : Attachment
    {
        [ForeignKey("DeliverableId")]
        public virtual Deliverable Deliverable { get; set; }

        public int DeliverableId { get; set; }
    }
}
