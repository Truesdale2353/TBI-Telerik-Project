using System;
using System.Collections.Generic;
using System.Text;

namespace TBIProject.Services.Models
{
   public class EmailUpdateModel
    {
        public int EmailId { get; set; }
        public string NewStatus { get; set; }
        public string PhoneNumber { get; set; }
        public string EGN { get; set; }
        public string LoggedUserUsername { get; set; }     
    }
}
