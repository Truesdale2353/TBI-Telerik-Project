using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBIProject.Data;
using TBIProject.Data.Models;
using TBIProject.Data.Models.Enums;
using TBIProject.Services.Contracts;
using TBIProject.Services.Models;

namespace TBIProject.Services.Implementation
{
    public class EmailListService : IEmailListService
    {
        private TBIContext context { get; }
        public EmailListService(TBIContext context)
        {
            this.context = context;
        }
        public async Task<List<EmailServiceModel>> ListEmails(int filter)
        {
            var app = new List<Application>();
            if (filter!=0)
            {
            app = context.Applications.Where(f=>f.ApplicationStatus==(ApplicationStatus)filter).ToList();
            }
            else
            {
                app = context.Applications.ToList();
            }
                


            var applications = app.Select(b => new EmailServiceModel
            {
                EmailId = b.Id,
                Emailreceived = b.Received,
                EmailSender = b.Email,
                EmailStatus = b.ApplicationStatus,
                OperatorId = b.OperatorId

            });
            return applications.ToList();
        }
    }
}
