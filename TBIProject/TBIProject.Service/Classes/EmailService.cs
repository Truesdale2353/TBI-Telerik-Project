using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBIProject.Data;
using TBIProject.Data.Models;
using TBIProject.Data.Models.Enums;
using TBIProject.Service.Interfaces;
using TBIProject.Service.Models;

namespace TBIProject.Service.Classes
{
    public class EmailService : IEmailService
    {
        private TBIContext context { get; }
        public EmailService(TBIContext context)
        {
            this.context = context;
        }

        public async Task<List<EmailServiceModel>> ListEmails(int filter)
        {            
            var emailList = context.Applications.ToList();           
            
     
            var applications = emailList.Select(b => new EmailServiceModel
            {
                EmailId = b.Id,
                Emailreceived = b.Received,
                EmailSender = b.Email,
                EmailStatus = b.ApplicationStatus,
                OperatorId = b.OperatorId

            }) ;
            return applications.ToList();
        }
    }
}
