using System;
using System.Collections.Generic;
using System.Text;

namespace TBIProject.Data.Models
{
    public class Loan
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        public decimal Amount { get; set; }

        public bool IsDeleted { get; set; }
    }
}
