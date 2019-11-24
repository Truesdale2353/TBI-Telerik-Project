using Microsoft.EntityFrameworkCore;
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
using TBIProject.Services.Providers.Encryption;

namespace TBIProject.Services.Implementation
{
    public class EmailListService : IEmailListService
    {
        private readonly TBIContext context;
        private readonly IEncrypter encrypter;

        public EmailListService(TBIContext context, IEncrypter encrypter)
        {
            this.context = context;
            this.encrypter = encrypter;
        }
        public async Task<List<EmailServiceModel>> ListEmails(int filter,int multyplier)

        {
            var currentListedEmails = (multyplier - 1) * 10;
            var emailsToBeListed = multyplier * 10;
            var app = new List<Application>();

            if (filter!=0)
            {
                // app = context.Applications.Where(f=>f.ApplicationStatus==(ApplicationStatus)filter).ToList();
                app = context.Applications.Where(f => f.ApplicationStatus == (ApplicationStatus)filter)
                    .OrderBy(d=>d.Id).Skip(currentListedEmails).Take(emailsToBeListed).ToList();
            }
            else
            {
                app = context.Applications.OrderBy(d => d.Id).Skip(currentListedEmails).Take(emailsToBeListed).ToList();
            }               

            var applications = app.Select(b => new EmailServiceModel
            {
                EmailId = b.Id,
                Emailreceived = b.Received,
                EmailSender = encrypter.Decrypt(b.Email),
                EmailStatus = b.ApplicationStatus,
                Attachments=b.AttachmentsCount,
                
            });
            return applications.ToList();
        }

        public async Task AddNewlyReceivedMessage(string gmailId, string body, string senderEmail, ICollection<int> attachmentData)
        {
            if (await this.context.Applications.AnyAsync(u => u.GmailId == gmailId)) return;

            var app = new Application
            {
                GmailId = gmailId,
                Body = encrypter.Encrypt(body),
                Email = encrypter.Encrypt(senderEmail),
                Received = DateTime.UtcNow,
                ApplicationStatus = ApplicationStatus.NotReviewed,
                LastChange = DateTime.UtcNow,
                AttachmentSize = attachmentData.Take(1).First(),
                AttachmentsCount = attachmentData.Skip(1).Take(1).First()
            };

            await this.context.Applications.AddAsync(app);

            await this.context.SaveChangesAsync();
        }
    }
}
