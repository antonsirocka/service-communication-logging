using LogsDataAccess.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommunicationLogging.Middleware.DisplayLogsMiddleware
{
    public class DisplayLogsMiddleware
    {
        private readonly DisplayLogsOptions _options;
        private readonly RequestDelegate _next;

        public DisplayLogsMiddleware(RequestDelegate next, DisplayLogsOptions options)
        {
            this._next = next;
            this._options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            var logPersister = context.RequestServices.GetService(typeof(ILogPersister));

            if (!context.Request.Path.StartsWithSegments(_options.Path))
            {
                await _next(context);
                return;
            }

            var model = new DisplayLogsModel()
            {
                Options = _options,
                Path = _options.Path
            };

            var logPage = new DisplayLogs(model, (ILogPersister)logPersister);

            await logPage.ExecuteAsync(context);
        }
    }
}
