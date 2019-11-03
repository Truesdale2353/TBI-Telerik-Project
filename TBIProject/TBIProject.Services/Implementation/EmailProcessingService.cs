using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TBIProject.Data;
using TBIProject.Data.Models;
using TBIProject.Services.Models;
using TBIProject.Services.Providers.Encryption;

namespace TBIProject.Services.Implementation
{
    public class EmailProcessingService : IEmailProcessingService
    {
        private TBIContext context;
        private IEncrypter encrypter;
        public EmailProcessingService(TBIContext context, IEncrypter encrypter)
        {
            this.context = context;
            this.encrypter = encrypter;
        }

        public async Task<FullEmailServiceModel> GetEmailFullInfo(int emailID)
        {
            var applicationEmail = await context.Applications.FindAsync(emailID);

            var serviceApplicationEmail = new FullEmailServiceModel
            {
                EmailId = applicationEmail.Id,
                Emailreceived = applicationEmail.Received,
                EmailSender = applicationEmail.Email,
                EmailStatus = applicationEmail.ApplicationStatus,
                Body = encrypter.Decrypt(applicationEmail.Body),
                OperatorId = applicationEmail.OperatorId


            };

            return serviceApplicationEmail;
        }

    }
}
