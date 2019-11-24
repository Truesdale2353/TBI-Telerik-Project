using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TBIProject.Models.EmailModels
{
    public class EmailAppendViewModel
    {
        public int EmailId { get; set; }
        public string Emailreceived { get; set; }
        public string EmailSender { get; set; }
        public string EmailStatus { get; set; }
        public int Attachments { get; set; }
    }
}
