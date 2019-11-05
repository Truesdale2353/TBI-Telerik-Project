using System;
using System.Collections.Generic;
using System.Text;
using TBIProject.Data.Models.Enums;

namespace TBIProject.Services.Models
{
   public class FullEmailServiceModel
    {
        public int EmailId { get; set; }
        public DateTime Emailreceived { get; set; }
        public string EmailSender { get; set; }
        public ApplicationStatus EmailStatus { get; set; }
        public string Body { get; set; }
        public string OperatorId { get; set; }
        public List<string> PermitedOperations { get; set; }
        public bool AllowedToWork { get; set; }
    }
}
