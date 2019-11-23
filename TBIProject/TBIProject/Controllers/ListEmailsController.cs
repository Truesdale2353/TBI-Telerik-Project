﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TBIProject.Data.Models;
using TBIProject.Data.Models.Enums;
using TBIProject.Models.EmailModels;
using TBIProject.Services.Contracts;
using TBIProject.Services.Implementation;

namespace TBIProject.Controllers
{
    public class ListEmailsController : Controller
    {
        private IEmailListService service { get; }
    
        public ListEmailsController(IEmailListService service)
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
            var emailListModel = new EmailListModel(applications.ToList());
            var stringedEnum = (ApplicationStatus)filter;
            emailListModel.currentSearchFilter = stringedEnum.ToString();


            return View(emailListModel);
        }

    }
}