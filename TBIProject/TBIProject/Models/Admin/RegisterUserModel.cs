using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TBIProject.Models.Admin
{
    public class RegisterUserModel
    {
        [EmailAddress]
        public string Email { get; set; }

        public string RoleId { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}
