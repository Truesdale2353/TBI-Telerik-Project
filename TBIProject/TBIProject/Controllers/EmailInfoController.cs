using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TBIProject.Controllers
{
    public class EmailInfoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetEmailInfo(int emailId)
        {
           
            return View();
        }

    }
}