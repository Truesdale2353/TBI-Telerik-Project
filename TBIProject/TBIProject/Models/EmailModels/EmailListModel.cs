using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TBIProject.Models.EmailModels
{
    public class EmailListModel
    {
        public string currentSearchFilter { get; set; }
        public ICollection<EmailViewModel> emailList { get; set; }

        public EmailListModel(ICollection<EmailViewModel> emailList)
        {
            this.emailList = emailList;
        }
    }
}
