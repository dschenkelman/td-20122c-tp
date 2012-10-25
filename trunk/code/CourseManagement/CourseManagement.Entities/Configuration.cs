using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace CourseManagement.Model
{
    public class Configuration
    {
        [Key]
        public string Protocol { get; set; }

        public string Endpoint { get; set; }

        public int Port { get; set; }

        public bool UseSsl { get; set; }

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        public int AccountId { get; set; }
    }
}
