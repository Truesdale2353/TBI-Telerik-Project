using Google.Apis.Gmail.v1.Data;
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
        private Timer timer;

        public EmailFetchingHostedService(IEmailService service)
        {
            this.service = service;
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

            if (messages.Count > 0) Console.WriteLine("JUICY MASSAGES");
        }

        private async Task<ICollection<Message>> GetNewMessagesAsync()
        {
            var messages = await this.service.GetMessagesAsync("is:unread", EmailConstants.EMAIL_ADDRESS);

            return messages.ToList();
        }
    }
}
