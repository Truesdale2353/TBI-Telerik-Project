using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBIProject.Data.Models.Enums;

namespace TBIProject.Models.EmailModels
{
    public class EmailFullInfoModel
    {
        public int EmailId { get; set; }
        public DateTime Emailreceived { get; set; }
        public string EmailSender { get; set; }
        public ApplicationStatus EmailStatus { get; set; }
        public string Body { get; set; }
        public string OperatorId { get; set; }
    }
}
