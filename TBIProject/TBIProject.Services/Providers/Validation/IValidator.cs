using System.Threading.Tasks;

namespace TBIProject.Services.Providers.Validation
{
    public interface IValidator
    {
        Task<bool> ValidatePhone(string phone);
        Task<bool> ValidateEGN(string egn);
        Task<bool> ValidateName(string fullName);
       

    }
}