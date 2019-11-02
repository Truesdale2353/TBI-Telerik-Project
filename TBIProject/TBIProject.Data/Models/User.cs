﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace TBIProject.Data.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }

        public string EGN { get; set; }

        public ICollection<Loan> Loans { get; set; }
    }
}
