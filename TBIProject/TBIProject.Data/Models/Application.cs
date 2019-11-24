using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TBIProject.Data.Models.Enums;

namespace TBIProject.Data.Models
{
    public class Application
    {
        public int Id { get; set; }
        public string OperatorId { get; set; }

        public User CurrentlyOpenedBy { get; set; }

        public ApplicationStatus ApplicationStatus { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string GmailId { get; set; }

        public string Body { get; set; }

        public int AttachmentsCount { get; set; }

        public int AttachmentSize { get; set; }

        public string EGN { get; set; }

        public string Phone { get; set; }

        public DateTime Received { get; set; }

        public DateTime LastChange { get; set; }
    }
}
