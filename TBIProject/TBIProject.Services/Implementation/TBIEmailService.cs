using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TBIProject.Services.Contracts;

namespace TBIProject.Services.Implementation
{
    public class TBIEmailService : IEmailService
    {
        private readonly GmailService emailService;
        private const string emailAddress = "telerik.tbi@gmail.com";

        public TBIEmailService(GmailService emailService)
        {
            this.emailService = emailService;
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(string email = emailAddress)
        {
            var messages = await this.emailService.Users.Messages.List(email).ExecuteAsync();

            return messages.Messages;
        }

        public async Task<Message> GetMessageAsync(string messageId, string email = emailAddress)
        {
            var message = await this.emailService.Users.Messages.Get(email, messageId).ExecuteAsync();

            return message;
        }

        public async Task<Message> ModifyMessageAsync(string messageId, IList<string> labelsToAdd, IList<string> labelsToRemove, string email = emailAddress)
        {
            var mods = new ModifyMessageRequest();
            mods.AddLabelIds = labelsToAdd;
            mods.RemoveLabelIds = labelsToRemove;

            try
            {
                return await emailService.Users.Messages.Modify(mods, email, messageId).ExecuteAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }

            return null;
        }

        public async Task<string> GetMessageBodyAsync(Message message)
        {
            var base64Message = Convert.FromBase64String(message.Payload.Parts[1].Body.Data.Replace("-", "+").Replace("_", "/"));

            var resultMessage = Encoding.UTF8.GetString(base64Message);

            return resultMessage;
        }
    }
}
