using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBIProject.Common.Constants;
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
            if (message.Payload.Parts[0].Body.Data != null)
            {
                var base64Message = Convert.FromBase64String(message.Payload.Parts[0].Body.Data?.Replace("-", "+").Replace("_", "/"));

                var resultMessage = Encoding.UTF8.GetString(base64Message);

                return resultMessage;
            }
            else
            {
                var base64Message = Convert.FromBase64String(message.Payload.Parts[0].Parts[0].Body.Data?.Replace("-", "+").Replace("_", "/"));

                var resultMessage = Encoding.UTF8.GetString(base64Message);

                return resultMessage;
            }
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

        public async Task<Message> SendMessageAsync(string email, string message, string subject)
        {
            var newMessage = this.BuildMessage(email, message, subject);

            var encodedMessage = Base64UrlEncoder.Encode(newMessage.ToString());

            var sendMessage = new Message { Raw = encodedMessage };

            return await this.emailService.Users.Messages.Send(sendMessage, "telerik.tbi@gmail.com").ExecuteAsync();
        }

        private string BuildMessage(string email, string message, string subject)
        {
            var newMessage = new StringBuilder();
            newMessage.AppendLine($"To: {email}");
            newMessage.AppendLine($"From: telerik.tbi@gmail.com");
            newMessage.AppendLine($"Subject: {subject}");
            newMessage.AppendLine();
            newMessage.AppendLine(message);

            return newMessage.ToString();
        }

        public async Task<IEnumerable<int>> GetAttachmentsAsync(Message message)
        {
            var parts = message.Payload.Parts;

            var totalSize = 0;

            var countOfAttachmenets = 0;

            foreach (MessagePart part in parts)
            {
                if (!string.IsNullOrEmpty(part.Filename))
                {
                    var attId = part.Body.AttachmentId;
                    MessagePartBody attachPart = await this.emailService.Users.Messages.Attachments.Get(EmailConstants.EMAIL_ADDRESS, message.Id, attId).ExecuteAsync();

                    if (attachPart != null)
                    {
                        totalSize += (int)attachPart.Size;
                        countOfAttachmenets++;
                    }
                }
            }

            return new List<int> { totalSize, countOfAttachmenets };
        }
    }
}
