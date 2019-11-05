using System.Collections.Generic;
using System.Threading.Tasks;
using TBIProject.Data.Models;
using TBIProject.Data.Models.Enums;
using TBIProject.Services.Models;

namespace TBIProject.Services.Implementation
{
    public interface IEmailProcessingService
    {
        Task<FullEmailServiceModel> GetEmailFullInfo(int emailID,string currentUser);       
        Task<bool> ProcessEmailUpdate(EmailUpdateModel parameters);
       


    }
}