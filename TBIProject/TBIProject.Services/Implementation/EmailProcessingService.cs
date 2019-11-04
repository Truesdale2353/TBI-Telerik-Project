using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBIProject.Data;
using TBIProject.Data.Models;
using TBIProject.Data.Models.Enums;
using TBIProject.Services.Models;
using TBIProject.Services.Providers.Encryption;

namespace TBIProject.Services.Implementation
{
    public class EmailProcessingService : IEmailProcessingService
    {
        private TBIContext context;
        private IEncrypter encrypter;
        private UserManager<User> userManager;
        public EmailProcessingService(TBIContext context, IEncrypter encrypter, UserManager<User> userManager)
        {
            this.context = context;
            this.encrypter = encrypter;
            this.userManager = userManager;
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

        public async Task<bool> ProcessEmailUpdate(int emailId, string newStatus, string currentUsername)
        {
            var emailToBeUpdated = await context.Applications.FindAsync(emailId);
            var currentLoggedUser = await userManager.FindByNameAsync(currentUsername);
            var listOfPermittedStatuses = await ReturnPermitedUpdates(emailToBeUpdated.ApplicationStatus, currentLoggedUser);

            if (!listOfPermittedStatuses.Contains(newStatus)) return false;

            var permitionGranted = await IsTheLoggedUserPermitedToUpdateTheEmail(currentLoggedUser,emailToBeUpdated);
            if (permitionGranted)
            {
                await UpdateApplication(newStatus, emailToBeUpdated, currentLoggedUser);
            }

            return true;
        }
        public async Task<List<string>> ReturnPermitedUpdates(ApplicationStatus currentStatus, User currentUser)
        {

            if (await userManager.IsInRoleAsync(currentUser, "Manager"))
            {
                return Enum.GetNames(typeof(ApplicationStatus)).ToList();
            }
            if (await userManager.IsInRoleAsync(currentUser, "Operator"))
            {
                if (currentStatus == ApplicationStatus.NotReviewed)
                    return new List<string> { nameof(ApplicationStatus.InvalidApplication), nameof(ApplicationStatus.New) };

                if (currentStatus == ApplicationStatus.New)
                    return new List<string> { nameof(ApplicationStatus.Open) };

                if (currentStatus == ApplicationStatus.Open)
                    return new List<string> { nameof(ApplicationStatus.Closed) };
            }

            return null;
        }

        private async Task<bool> IsTheLoggedUserPermitedToUpdateTheEmail(User currentUser,Application selectedEmail)
        {
            if (await userManager.IsInRoleAsync(currentUser, "Manager")) return true;

            if (selectedEmail.CurrentlyOpenedBy==null||selectedEmail.CurrentlyOpenedBy==currentUser)
            {
                return true;
            }
            return false;
            

        }

        private async Task UpdateApplication(string newStatus, Application selectedEmail,User currentUser)
        {
            if (selectedEmail.CurrentlyOpenedBy==null&&selectedEmail.ApplicationStatus==ApplicationStatus.NotReviewed)
            {
                selectedEmail.CurrentlyOpenedBy = currentUser;
            }        

            ApplicationStatus myStatus;
            Enum.TryParse(newStatus, out myStatus);
            selectedEmail.ApplicationStatus = myStatus;
            // Log changes
            await context.SaveChangesAsync();
        }

    }
}
