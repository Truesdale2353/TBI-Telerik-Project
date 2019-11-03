using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TBIProject.Models.EmailModels;
using TBIProject.Service.Interfaces;

namespace TBIProject.Controllers
{
    public class ListEmailsController : Controller
    {
        private IEmailService service { get; }
        public ListEmailsController(IEmailService service)
        {
            this.service = service;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> BrowseEmails(int filter)
        {
            var emails = await service.ListEmails(filter);
            var applications = emails.Select(e => new EmailViewModel
            {
                EmailId = e.EmailId,
                Emailreceived = e.Emailreceived,
                EmailSender = e.EmailSender,
                EmailStatus = e.EmailStatus           
            }) ;

            return View(new EmailListModel(applications.ToList()));
        }

    }
}