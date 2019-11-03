using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBIProject.Data.Models.Enums;

namespace TBIProject.Models.EmailModels
{
    public class EmailViewModel
    {
        public int EmailId { get; set; }
        public DateTime Emailreceived { get; set; }
        public string EmailSender { get; set; }
        public ApplicationStatus EmailStatus { get; set; }
    }
}
