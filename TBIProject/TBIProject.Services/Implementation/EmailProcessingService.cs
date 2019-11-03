using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TBIProject.Data;
using TBIProject.Data.Models;

namespace TBIProject.Services.Implementation
{
   public class EmailProcessingService
    {
        private TBIContext context;
        public EmailProcessingService(TBIContext context)
        {
            this.context = context;
        }

        public async Task<Application>GetEmailFullInfo(int emailID)
        {
            var applicationEmail = await context.Applications.FindAsync(emailID);
            
            return null;
        }

    }
}
