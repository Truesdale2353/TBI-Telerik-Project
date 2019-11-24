using System;
using System.Collections.Generic;
using System.Text;
using TBIProject.Data.Models.Enums;

namespace TBIProject.Services.Models
{
  public  class EmailServiceModel
    {
        public int EmailId { get; set; }
        public DateTime Emailreceived { get; set; }
        public string EmailSender { get; set; }
        public ApplicationStatus EmailStatus { get; set; }
        public int Attachments { get; set; }
    }
}
