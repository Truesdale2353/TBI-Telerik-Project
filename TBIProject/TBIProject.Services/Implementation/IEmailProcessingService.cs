using System.Threading.Tasks;
using TBIProject.Services.Models;

namespace TBIProject.Services.Implementation
{
    public interface IEmailProcessingService
    {
        Task<FullEmailServiceModel> GetEmailFullInfo(int emailID);

         Task<bool> ProcessEmailUpdate(int emailId, string newStatus, string currentUsername);
    }
}