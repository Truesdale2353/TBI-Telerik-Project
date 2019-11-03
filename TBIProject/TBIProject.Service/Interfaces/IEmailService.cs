using System.Collections.Generic;
using System.Threading.Tasks;
using TBIProject.Data.Models;
using TBIProject.Service.Models;

namespace TBIProject.Service.Interfaces
{
    public interface IEmailService
    {
        Task<List<EmailServiceModel>> ListEmails(int filter);
    }
}