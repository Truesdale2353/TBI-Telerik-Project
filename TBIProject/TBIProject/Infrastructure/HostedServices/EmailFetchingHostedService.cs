using Google.Apis.Gmail.v1.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TBIProject.Common.Constants;
using TBIProject.Services.Contracts;

namespace TBIProject.Infrastructure.HostedServices
{
    public class EmailFetchingHostedService : IHostedService, IDisposable
    {
        private readonly IEmailService service;
        private readonly IServiceScopeFactory scopeFactory;
        private Timer timer;

        public EmailFetchingHostedService(IEmailService service, IServiceScopeFactory scopeFactory)
        {
            this.service = service;
            this.scopeFactory = scopeFactory;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var messages =  this.GetNewMessagesAsync().GetAwaiter().GetResult();

            if (messages.Count > 0)
            {
                using (var scope = this.scopeFactory.CreateScope())
                {
                    var emailListService = scope.ServiceProvider.GetRequiredService<IEmailListService>();
                    foreach (var messageData in messages)
                    {
                        var message = this.service.GetMessageAsync(messageData.Id, EmailConstants.EMAIL_ADDRESS).GetAwaiter().GetResult();

                        var body = this.service.GetMessageBodyAsync(message).GetAwaiter().GetResult();

                        var senderEmail = this.service.GetMessageEmailSenderAsync(message).GetAwaiter().GetResult();

                        emailListService.AddNewlyReceivedMessage(message.Id, body, senderEmail);

                        this.service.ModifyMessageAsync(message.Id, null, new List<string> { "UNREAD" }, EmailConstants.EMAIL_ADDRESS);
                    }
                }
               
            }
        }

        private async Task<ICollection<Message>> GetNewMessagesAsync()
        {
            var messages = await this.service.GetMessagesAsync("is:unread",EmailConstants.EMAIL_ADDRESS);

            return messages.ToList();
        }
    }
}
