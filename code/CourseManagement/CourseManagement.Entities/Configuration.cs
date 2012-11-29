namespace CourseManagement.Model
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Configuration
    {
        [Key]
        public string Protocol { get; set; }

        public string Endpoint { get; set; }

        public int Port { get; set; }

        [DisplayName("Uses Ssl")]
        public bool UseSsl { get; set; }

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        public int AccountId { get; set; }
    }
}
