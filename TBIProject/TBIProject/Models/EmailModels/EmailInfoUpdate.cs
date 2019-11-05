using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TBIProject.Models.EmailModels
{
    public class EmailInfoUpdate
    {
        [Required]
        public int EmailId { get; set; }
        [Required]
        public string NewStatus { get; set; }
    }
}
