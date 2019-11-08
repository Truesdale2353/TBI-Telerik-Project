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
using TBIProject.Services.Providers.Validation;

namespace TBIProject.Services.Implementation
{
    public class EmailProcessingService : IEmailProcessingService
    {
        private TBIContext context;
        private IEncrypter encrypter;
        private UserManager<User> userManager;
        private IValidator validator;
        public EmailProcessingService(TBIContext context, IEncrypter encrypter, UserManager<User> userManager,IValidator validator)
        {
            this.context = context;
            this.encrypter = encrypter;
            this.userManager = userManager;
            this.validator = validator;
        }

        public async Task<FullEmailServiceModel> GetEmailFullInfo(int emailID,string userName)
        {
            var applicationEmail = await context.Applications.FindAsync(emailID);
            var currentUser = await userManager.FindByNameAsync(userName);
            var permittedOp = await ReturnPermitedUpdates(applicationEmail.ApplicationStatus, currentUser);
            var allowedToWork = await IsTheLoggedUserPermitedToUpdateTheEmail(currentUser,applicationEmail);

            var serviceApplicationEmail = new FullEmailServiceModel
            {
                EmailId = applicationEmail.Id,
                Emailreceived = applicationEmail.Received,
                EmailSender = applicationEmail.Email,
                EmailStatus = applicationEmail.ApplicationStatus,
                Body = encrypter.Decrypt(applicationEmail.Body),
                OperatorId=applicationEmail.OperatorId,
                PermitedOperations = permittedOp,
                AllowedToWork = allowedToWork
            };

            return serviceApplicationEmail;
        }

        public async Task<bool> ProcessEmailUpdate(EmailUpdateModel parameters)
        {
            var emailToBeUpdated = await context.Applications.FindAsync(parameters.EmailId);
            var currentLoggedUser = await userManager.FindByNameAsync(parameters.LoggedUserUsername);
            var listOfPermittedStatuses = await ReturnPermitedUpdates(emailToBeUpdated.ApplicationStatus, currentLoggedUser);

            if (!listOfPermittedStatuses.Contains(parameters.NewStatus)) return false;

            var permitionGranted = await IsTheLoggedUserPermitedToUpdateTheEmail(currentLoggedUser,emailToBeUpdated);
            if (permitionGranted)
            {
                await UpdateApplication(parameters.NewStatus, emailToBeUpdated, currentLoggedUser);
                if (parameters.NewStatus== ApplicationStatus.Open.ToString())
                {
                var success = await UpdateApplicationProperties(parameters.PhoneNumber, parameters.EGN, emailToBeUpdated);
                if (!success) return false;
                }
                emailToBeUpdated.LastChange = DateTime.Now;
                await context.SaveChangesAsync();
            }

            return true;
        }
        private async Task<List<string>> ReturnPermitedUpdates(ApplicationStatus currentStatus, User currentUser)
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

            if (selectedEmail.OperatorId == null||selectedEmail.OperatorId==currentUser.Id)
            {
                return true;
            }
            return false;
            

        }

        private async Task UpdateApplication(string newStatus, Application selectedEmail,User currentUser)
        {
            if (selectedEmail.OperatorId==null&&selectedEmail.ApplicationStatus==ApplicationStatus.NotReviewed)
            {
                selectedEmail.OperatorId = currentUser.Id;
            }        

            ApplicationStatus myStatus;
            Enum.TryParse(newStatus, out myStatus);
            selectedEmail.ApplicationStatus = myStatus;
            // Log changes
            await context.SaveChangesAsync();
        }

        private async Task<bool> UpdateApplicationProperties(string phone,string egn,Application app)
        {
            var isEgnValid=await validator.ValidateEGN(egn);
            var isPhoneValid = await validator.ValidatePhone(phone);
            if (isEgnValid&&isPhoneValid)
            {
                app.EGN = encrypter.Encrypt(egn);
                app.Phone = encrypter.Encrypt(phone);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }
       

    }
}
