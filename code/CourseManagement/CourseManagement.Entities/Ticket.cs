using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagement.Model
{
    public class Ticket
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string EmailSubject { get; set; }
        
        [Required]
        public string EmailBody { get; set; }

        [Required]
        public bool IsPrivate { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }

        [Required]
        public TicketState State { get; set; }

        public int TeacherId { get; set; }

        public int StudentId { get; set; }
        
        [ForeignKey("TeacherId")]
        public virtual Teacher AssignedTeacher { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Creator { get; set; }

        public virtual ICollection<Reply> Replies { get; set; }
    }
}
