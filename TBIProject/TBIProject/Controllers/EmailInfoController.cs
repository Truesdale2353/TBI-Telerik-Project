using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
            var applicationEmail = await processingService.GetEmailFullInfo(emailId);

            var applicationEmailView = new EmailFullInfoModel
            {
                EmailId = applicationEmail.EmailId,
                Emailreceived = applicationEmail.Emailreceived,
                EmailSender=applicationEmail.EmailSender,
                EmailStatus=applicationEmail.EmailStatus,
                Body= applicationEmail.Body,
                OperatorId= applicationEmail.OperatorId

            };

            return View(applicationEmailView);
        }

        public async Task<IActionResult> UpdateEmail(int emailId,string newStatus)
        {
            var loggedUserUsername = User.Identity.Name;
        var success=   await processingService.ProcessEmailUpdate(emailId, newStatus, loggedUserUsername);
            return null;
        }



    }
}