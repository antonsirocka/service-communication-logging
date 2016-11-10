using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using LogsDataAccess.Interfaces;
using LogsDataAccess;

namespace ServiceCommunicationLogging.Middleware
{
    public class LoggingMiddleware
    {
        private RequestDelegate _next;        
        private ILoggerFactory _loggerFactory;
        private string _serviceName;

        public LoggingMiddleware(RequestDelegate next, string serviceName, ILoggerFactory loggerFactory)
        {
            _next = next;
            _loggerFactory = loggerFactory;
            _serviceName = serviceName;
        }

        public async Task Invoke(HttpContext context)
        {
            ILogPersister logPersister = (ILogPersister)context.RequestServices.GetService(typeof(ILogPersister));

            var traceStream = new MemoryStream();

            _loggerFactory.AddTraceSource(
                new SourceSwitch("AspNet5LoggingSwitch", "Verbose"),
                new TextWriterTraceListener(traceStream, "MiddlewareTraceListener")
            );

            // declare request variables
            var requestBodyStream = new MemoryStream();
            var requestBody = context.Request.Body;

            // make a copy of the request stream
            await context.Request.Body.CopyToAsync(requestBodyStream);

            // rewing the stream
            requestBodyStream.Seek(0, SeekOrigin.Begin);

            // read the stream into string
            var requestBodyAsString = await new StreamReader(requestBodyStream).ReadToEndAsync();

            // save request
            var serviceMessageRequest = this.CreateRequest(requestBodyAsString, context);
            await logPersister.Save(serviceMessageRequest);

            // rewing the stream
            requestBodyStream.Seek(0, SeekOrigin.Begin);

            // copy stream back into original place
            context.Request.Body = requestBodyStream;

            // declare response variables
            var responseBodyStream = new MemoryStream();
            var responseBody = context.Response.Body;
            context.Response.Body = responseBodyStream;

            // pass request to next middleware and await response
            await _next.Invoke(context);

            // rewind the stream
            responseBodyStream.Seek(0, SeekOrigin.Begin);

            // read stream into string
            string responseBodyAsString = await new StreamReader(responseBodyStream).ReadToEndAsync();

            // rewing stream
            responseBodyStream.Seek(0, SeekOrigin.Begin);

            // write stream back into original place
            await responseBodyStream.CopyToAsync(responseBody);
            context.Response.Body = responseBody;

            // read trace
            traceStream.Seek(0, SeekOrigin.Begin);
            var trace = await new StreamReader(traceStream).ReadToEndAsync();

            // save response
            var serviceMessageResponse = this.CreateResponse(serviceMessageRequest, responseBodyAsString, trace);
            await logPersister.Save(serviceMessageRequest, true);

            if (Trace.Listeners.Count > 0)
            {
                Trace.Listeners[0].Dispose();
            }
        }

        private ServiceMessageLog CreateRequest(string requestBody, HttpContext context)
        {
            var serviceMessageLog = new ServiceMessageLog()
            {
                Id = Guid.NewGuid(),
                ContextId = Guid.Parse(context.Session.Id),
                RequestCreatedOn = DateTime.Now,
                RequestXml = requestBody,
                ServiceName = _serviceName,
                ServicePath = context.Request.Path.HasValue ? context.Request.Path.Value : string.Empty,
                HostName = context.Request.Host.HasValue ? context.Request.Host.Value : string.Empty,
                ServiceMethod = context.Request.Method
            };

            return serviceMessageLog;
        }

        private ServiceMessageLog CreateResponse(ServiceMessageLog request, string responseBody, string trace)
        {
            request.ResponseCreatedOn = DateTime.Now;
            request.ResponseXml = responseBody;
            request.ServiceTrace = trace;

            //var response = new ServiceMessageLog()
            //{
            //    Id = request.Id,
            //    ContextId = request.ContextId,
            //    ResponseCreatedOn = DateTime.Now,
            //    ResponseXml = responseBody,
            //    ServiceTrace = trace
            //};

            return request;
        }
    }
}
