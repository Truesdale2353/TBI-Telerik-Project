using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBIProject.Data;
using TBIProject.Data.Models;

namespace TBIProject.Infrastructure.Middlewares
{
    public class ChangePasswordMiddleware
    {
        private readonly RequestDelegate next;
        private const string RedirectPath = "/Identity/Account/Manage/ChangePassword";

        public ChangePasswordMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var user = context.User;

            if (user.Identity.IsAuthenticated)
            {
                var userManager = (UserManager<User>)context.RequestServices.GetService(typeof(UserManager<User>));
                var username = user.Identity.Name;

                var loggedUser = await userManager.FindByEmailAsync(username);

                if (!loggedUser.HasChangedPassword)
                {
                    if (context.Request.Path != RedirectPath) context.Response.Redirect(RedirectPath);
                }
            }

            await next(context);
        }
    }
}
