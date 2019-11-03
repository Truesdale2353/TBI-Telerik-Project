using System.Collections.Generic;
using System.Threading.Tasks;
using TBIProject.Services.Models;

namespace TBIProject.Services.Contracts
{
    public interface IEmailListService
    {
        Task<List<EmailServiceModel>> ListEmails(int filter);
    }
}