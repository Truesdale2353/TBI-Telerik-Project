using System;
using System.Collections.Generic;
using System.Text;
using TBIProject.Data;

namespace TBIProject.Services.Implementation
{
   public class EmailProcessingService
    {
        private TBIContext context;
        public EmailProcessingService(TBIContext context)
        {
            this.context = context;
        }

    }
}
