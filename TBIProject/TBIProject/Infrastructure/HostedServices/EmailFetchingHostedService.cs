using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TBIProject.Services.Contracts;

namespace TBIProject.Infrastructure.HostedServices
{
    public class EmailFetchingHostedService : IHostedService
    {
        private readonly IEmailService service;

        public EmailFetchingHostedService(IEmailService service)
        {
            this.service = service;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
