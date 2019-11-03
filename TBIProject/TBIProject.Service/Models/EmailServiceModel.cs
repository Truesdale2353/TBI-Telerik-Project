using System;
using TBIProject.Data.Models.Enums;

namespace TBIProject.Service.Models
{
    public class EmailServiceModel
    {
        public int EmailId { get; set; }
        public DateTime Emailreceived { get; set; }
        public string EmailSender{ get; set; }
        public ApplicationStatus EmailStatus { get; set; }
        public string OperatorId{ get; set; }


      
    }
}
