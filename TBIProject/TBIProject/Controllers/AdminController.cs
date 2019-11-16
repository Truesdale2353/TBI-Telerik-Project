using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBIProject.Data.Models;
using TBIProject.Models.Admin;
using System.Web;

namespace TBIProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<User> userManager;
        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        public async Task<IActionResult> RegisterUser()
        {
            var roles = await this.roleManager.Roles.Where(r => r.Name != "Admin").Select(r => new SelectListItem { Text = r.Name, Value = r.Id }).ToListAsync();

            return View(new RegisterUserModel { Roles = roles });
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await this.userManager.FindByEmailAsync(model.Email);

            if(user == null)
            {
                user = new User
                {
                    Email = model.Email,
                    UserName = model.Email
                };
                var result = await this.userManager.CreateAsync(user, await this.GetPassword(model.Email));

                if (!result.Succeeded)
                {
                    return View(model);
                }
                
            }

            return RedirectToAction("Index", "Home");
        }

        private async Task<string> GetPassword(string email)
        {
            if(string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("Email cannot be null!");
            }

            var firstPart = email.Split("@");

            if (firstPart.Length <= 1) throw new ArgumentOutOfRangeException("Not a valid email!");

            var secondPart = firstPart[1].Split(".");

            var password = firstPart[0] + secondPart[0] + "1234" + Char.ToUpper(firstPart[0][0]) + "$";

            return password;
        }
    }
}
