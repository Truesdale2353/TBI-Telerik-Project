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
        private readonly TBIContext context;
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

        public async Task AddNewlyReceivedMessage(string gmailId, string body)
        {
            var app = new Application
            {
                GmailId = gmailId,
                Body = body,
                Received = DateTime.UtcNow,
                ApplicationStatus = ApplicationStatus.NotReviewed
            };

            await this.context.Applications.AddAsync(app);

            await this.context.SaveChangesAsync();
        }
    }
}
