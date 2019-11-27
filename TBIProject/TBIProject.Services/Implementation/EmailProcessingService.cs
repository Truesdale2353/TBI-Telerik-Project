using Microsoft.AspNetCore.Identity;
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
using TBIProject.Services.Providers.Validation;

namespace TBIProject.Services.Implementation
{
    public class EmailProcessingService : IEmailProcessingService
    {
        private TBIContext context;
        private IEncrypter encrypter;
        private UserManager<User> userManager;
        private IValidator validator;
        private readonly IEmailService emailService;
        public EmailProcessingService(TBIContext context, IEncrypter encrypter, UserManager<User> userManager, IValidator validator, IEmailService emailService)
        {
            this.context = context;
            this.encrypter = encrypter;
            this.userManager = userManager;
            this.validator = validator;
            this.emailService = emailService;
        }

        public async Task<FullEmailServiceModel> GetEmailFullInfo(int emailID, string userName)
        {
            var applicationEmail = await context.Applications.FindAsync(emailID);
            var currentUser = await userManager.FindByNameAsync(userName);
            var permittedOp = await ReturnPermitedUpdates(applicationEmail.ApplicationStatus, currentUser);
            var allowedToWork = await IsTheLoggedUserPermitedToUpdateTheEmail(currentUser, applicationEmail);
            var serviceApplicationEmail = new FullEmailServiceModel
            {
                EmailId = applicationEmail.Id,
                Emailreceived = applicationEmail.Received,
                EmailSender = encrypter.Decrypt(applicationEmail.Email),
                EmailStatus = applicationEmail.ApplicationStatus,
                Body = encrypter.Decrypt(applicationEmail.Body),
                OperatorId = applicationEmail.OperatorId,
                PermitedOperations = permittedOp,
                AllowedToWork = allowedToWork,
                CurrentDataStamp = applicationEmail.LastChange.Ticks.ToString()

            };

            return serviceApplicationEmail;
        }
        public async Task<bool> ValidateEmailTimeStamp(int emailId, string emailStamp)
        {
            var emailToBeUpdated = await context.Applications.FindAsync(emailId);

            if (emailToBeUpdated.LastChange.Ticks.ToString() != emailStamp)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> ProcessEmailUpdate(EmailUpdateModel parameters)
        {
            var emailToBeUpdated = await context.Applications.FindAsync(parameters.EmailId);
            var currentLoggedUser = await userManager.FindByNameAsync(parameters.LoggedUserUsername);
            var listOfPermittedStatuses = await ReturnPermitedUpdates(emailToBeUpdated.ApplicationStatus, currentLoggedUser);

            if (!listOfPermittedStatuses.Contains(parameters.NewStatus)) return false;

            var permitionGranted = await IsTheLoggedUserPermitedToUpdateTheEmail(currentLoggedUser, emailToBeUpdated);
            if (permitionGranted)
            {
                await UpdateApplication(parameters.NewStatus, emailToBeUpdated, currentLoggedUser);
                if (parameters.NewStatus == ApplicationStatus.Open.ToString())
                {
                    var success = await UpdateApplicationProperties(parameters.PhoneNumber, parameters.EGN, emailToBeUpdated);

                    if (!success) return false;
                }
                if (emailToBeUpdated.ApplicationStatus == ApplicationStatus.Accepted)
                {
                    if (!await validator.ValidateName(parameters.FullName))
                    {
                        return false;
                    }
                    await this.IssueAccount(emailToBeUpdated.Email, emailToBeUpdated.Phone, emailToBeUpdated.EGN, parameters.FullName);
                    await this.IssueLoan(emailToBeUpdated.Email, parameters.Amount);
                }

                emailToBeUpdated.LastChange = DateTime.Now;
                await context.SaveChangesAsync(currentLoggedUser);
                return true;
            }

            return false;
        }

        private async Task IssueLoan(string email, decimal amount)
        {
            var decryptedEmail = this.encrypter.Decrypt(email).TrimStart('<').TrimEnd('>');

            var user = await this.userManager.FindByEmailAsync(decryptedEmail);

            if (user == null) throw new ArgumentException("No such user found!");

            var loan = new Loan
            {
                Amount = amount,
                UserId = user.Id,
                User = user,
                IsDeleted = false
            };

            await this.context.Loans.AddAsync(loan);

            await this.context.SaveChangesAsync();
        }

        private async Task<List<string>> ReturnPermitedUpdates(ApplicationStatus currentStatus, User currentUser)
        {

            if (await userManager.IsInRoleAsync(currentUser, "Manager"))
            {
                if (currentStatus == ApplicationStatus.NotReviewed)
                    return new List<string> { nameof(ApplicationStatus.InvalidApplication), nameof(ApplicationStatus.New) };

                if (currentStatus == ApplicationStatus.New)
                    return new List<string> { nameof(ApplicationStatus.Open) };

                if (currentStatus == ApplicationStatus.Open)
                    return new List<string> { nameof(ApplicationStatus.Accepted), nameof(ApplicationStatus.Rejected) };

                if (currentStatus == ApplicationStatus.Rejected)
                    return new List<string> { nameof(ApplicationStatus.New) };

                if (currentStatus == ApplicationStatus.Accepted)
                    return new List<string> { nameof(ApplicationStatus.New) };
            }
            if (await userManager.IsInRoleAsync(currentUser, "Operator"))
            {
                if (currentStatus == ApplicationStatus.NotReviewed)
                    return new List<string> { nameof(ApplicationStatus.InvalidApplication), nameof(ApplicationStatus.New) };

                if (currentStatus == ApplicationStatus.New)
                    return new List<string> { nameof(ApplicationStatus.Open) };

                if (currentStatus == ApplicationStatus.Open)
                    return new List<string> { nameof(ApplicationStatus.Accepted), nameof(ApplicationStatus.Rejected)};
            }

            return null;
        }

        private async Task<bool> IsTheLoggedUserPermitedToUpdateTheEmail(User currentUser, Application selectedEmail)
        {
            if (await userManager.IsInRoleAsync(currentUser, "Manager")) return true;

            if (selectedEmail.OperatorId == null || selectedEmail.OperatorId == currentUser.Id)
            {
                return true;
            }
            return false;
        }

        private async Task UpdateApplication(string newStatus, Application selectedEmail, User currentUser)
        {
            if (selectedEmail.OperatorId == null && selectedEmail.ApplicationStatus == ApplicationStatus.NotReviewed)
            {
                selectedEmail.OperatorId = currentUser.Id;
            }

            ApplicationStatus myStatus;
            Enum.TryParse(newStatus, out myStatus);
            selectedEmail.ApplicationStatus = myStatus;

        }

        private async Task<bool> UpdateApplicationProperties(string phone, string egn, Application app)
        {
            var isEgnValid = await validator.ValidateEGN(egn);

            var isPhoneValid = await validator.ValidatePhone(phone);

            if (isEgnValid && isPhoneValid)
            {
                app.EGN = encrypter.Encrypt(egn);
                app.Phone = encrypter.Encrypt(phone);
                return true;
            }
            return false;
        }

        private async Task<bool> IssueAccount(string email, string phoneNumber, string egn, string fullName)
        {
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException("Not a valid email address");

            var exists = await this.userManager.FindByEmailAsync(email);

            if (exists != null) return true;

            var password = this.GeneratePassword();

            var decryptedEmail = this.encrypter.Decrypt(email).TrimStart('<').TrimEnd('>');

            var user = new User
            {
                Email = decryptedEmail,
                UserName = decryptedEmail,
                PhoneNumber = phoneNumber,
                EGN = egn,
                FullName = fullName,
                HasChangedPassword = false,
            };

            await this.userManager.CreateAsync(user, password);

            await this.context.SaveChangesAsync();

            await this.emailService.SendMessageAsync(decryptedEmail, $"You have been issued an account in our website. Please log in and change your passoword, otherwise you wont be able to use our system. Your username is: {decryptedEmail}, Your temporary password is: {password}", "Tbi telerik loan applcation");

            return true;
        }

        private string GeneratePassword()
        {
            var password = new StringBuilder(Guid.NewGuid().ToString());

            password.Append("$123");

            for (int i = 0; i < password.Length; i++)
            {
                if (Char.IsLetter(password[i]))
                {
                    password[i] = Char.ToUpper(password[i]);
                    break;
                }
            }

            return password.ToString();
        }

    }
}
