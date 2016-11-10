using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommunicationLogging.Middleware.DisplayLogsMiddleware
{
    public static class DisplayLogsExtensions
    {
        public static IApplicationBuilder UseDisplayLogsPage(this IApplicationBuilder app, DisplayLogsOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<DisplayLogsMiddleware>(options);
        }
    }
}
