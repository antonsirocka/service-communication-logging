using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommunicationLogging.Middleware.DisplayLogsMiddleware
{
    public class DisplayLogsModel
    {
        public DisplayLogsOptions Options { get; set; }

        public string Path { get; set; }
    }
}
