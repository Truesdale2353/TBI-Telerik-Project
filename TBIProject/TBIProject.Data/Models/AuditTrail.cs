using System;
using System.Collections.Generic;
using System.Text;

namespace TBIProject.Data.Models
{
    public class AuditTrail
    {
        public int Id { get; set; }

        public string Table { get; set; }

        public string Column { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public DateTime Date { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }
    }
}
