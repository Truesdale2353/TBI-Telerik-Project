using Google.Apis.Gmail.v1.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TBIProject.Services.Contracts
{
    public interface IEmailService
    {
        Task<IEnumerable<Message>> GetMessagesAsync(string email);

        Task<Message> GetMessageAsync(string messageId, string email);

        Task<IEnumerable<Message>> GetMessagesAsync(string query, string email);

        Task<Message> ModifyMessageAsync(string messageId, IList<string> labelsToAdd, IList<string> labelsToRemove, string email);

        Task<string> GetMessageBodyAsync(Message message);
    }
}
