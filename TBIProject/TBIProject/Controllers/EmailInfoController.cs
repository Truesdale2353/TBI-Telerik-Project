using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TBIProject.Data.Models;
using TBIProject.Models.EmailModels;
using TBIProject.Services.Implementation;

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
                PermitedOperations = applicationEmail.PermitedOperations

            };

            return View(applicationEmailView);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateEmail(EmailInfoUpdate emailUpdate)
        {
            var loggedUserUsername = User.Identity.Name;
            var success = await processingService.ProcessEmailUpdate(emailUpdate.EmailId, emailUpdate.NewStatus, loggedUserUsername);
            if (success)
            {
                return RedirectToAction("GetEmailInfo", "EmailInfo", new { emailId = emailUpdate.EmailId });
            }
            return BadRequest();
        }



    }
}