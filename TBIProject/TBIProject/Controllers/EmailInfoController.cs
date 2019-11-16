using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TBIProject.Data.Models;
using TBIProject.Models.EmailModels;
using TBIProject.Services.Implementation;
using TBIProject.Services.Models;

namespace TBIProject.Controllers
{
    public class EmailInfoController : Controller
    {

        private readonly IEmailProcessingService processingService;
        public EmailInfoController(IEmailProcessingService processingService)
        {
            this.processingService = processingService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetEmailInfo(int emailId)
        {
            var userName = User.Identity.Name;
            var applicationEmail = await processingService.GetEmailFullInfo(emailId, userName);

            var applicationEmailView = new EmailFullInfoModel
            {
                EmailId = applicationEmail.EmailId,
                Emailreceived = applicationEmail.Emailreceived,
                EmailSender = applicationEmail.EmailSender,
                EmailStatus = applicationEmail.EmailStatus,
                Body = applicationEmail.Body,
                OperatorId = applicationEmail.OperatorId,
                AllowedToWork = applicationEmail.AllowedToWork,
                PermitedOperations = applicationEmail.PermitedOperations,
                CurrentDataTimeStamp=applicationEmail.CurrentDataStamp

            };

            return View(applicationEmailView);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateEmail(EmailInfoUpdate emailUpdate)
        {

            var loggedUserUsername = User.Identity.Name;
            var updateParameters = new EmailUpdateModel
            {
                EGN = emailUpdate.EGN,
                EmailId = emailUpdate.EmailId,
                LoggedUserUsername = loggedUserUsername,
                NewStatus = emailUpdate.NewStatus,
                PhoneNumber = emailUpdate.PhoneNumber,              
                
            };
            var doWeUpdateValidData = await processingService.ValidateEmailTimeStamp(emailUpdate.EmailId, emailUpdate.CurrentDataStamp);
            if (!doWeUpdateValidData)
            {
                this.TempData["executionMessage"] = "The data you are trying to access has been changed, this is the latest version.";
                return RedirectToAction("GetEmailInfo", "EmailInfo", new { emailId = emailUpdate.EmailId });
            }
            var success = await processingService.ProcessEmailUpdate(updateParameters);     

            if (success)
            {
                this.TempData["executionMessage"] = "";
                return RedirectToAction("GetEmailInfo", "EmailInfo", new { emailId = emailUpdate.EmailId });
            }
            return BadRequest();
        }



    }
}