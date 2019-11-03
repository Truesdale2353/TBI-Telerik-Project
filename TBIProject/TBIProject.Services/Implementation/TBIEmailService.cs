using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBIProject.Services.Contracts;

namespace TBIProject.Services.Implementation
{
    public class TBIEmailService : IEmailService
    {
        private readonly GmailService emailService;

        public TBIEmailService(GmailService emailService)
        {
            this.emailService = emailService;
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(string email)
        {
            var messages = await this.emailService.Users.Messages.List(email).ExecuteAsync();

            return messages.Messages;
        }

        public async Task<Message> GetMessageAsync(string messageId, string email)
        {
            var message = await this.emailService.Users.Messages.Get(email, messageId).ExecuteAsync();

            return message;
        }

        public async Task<Message> ModifyMessageAsync(string messageId, IList<string> labelsToAdd, IList<string> labelsToRemove, string email)
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
            var base64Message = Convert.FromBase64String(message.Payload.Parts[0].Body.Data.Replace("-", "+").Replace("_", "/"));

            var resultMessage = Encoding.UTF8.GetString(base64Message);

            return resultMessage;
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(string query, string email)
        {
            List<Message> result = new List<Message>();
            UsersResource.MessagesResource.ListRequest request = this.emailService.Users.Messages.List(email);
            request.Q = query;

            do
            {
                try
                {
                    ListMessagesResponse response = await request.ExecuteAsync();
                    result.AddRange(response.Messages);
                    request.PageToken = response.NextPageToken;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                }
            } while (!String.IsNullOrEmpty(request.PageToken));

            return result;
        }

        public async Task<string> GetMessageEmailSenderAsync(Message message)
        {
            return message.Payload.Headers.FirstOrDefault(h => h.Name == "Return-Path").Value;
        }
    }
}
