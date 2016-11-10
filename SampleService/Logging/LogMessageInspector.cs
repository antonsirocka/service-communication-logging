namespace SampleService.Logging
{
    using System;
    using System.ServiceModel.Dispatcher;
    using LogsDataAccess;
    using LogsDataAccess.AzureImplementation;
    using LogsDataAccess.Interfaces;
    using System.Xml.XPath;

    /// <summary>
    /// Log Message Inspector
    /// </summary>
    public class LogMessageInspector : IDispatchMessageInspector
    {
        private readonly string systemName;

        /// <summary>
        /// Initialises a new instance of the <see cref="LogMessageInspector"/> class.
        /// </summary>
        /// <param name="systemName">Name of the system.</param>
        public LogMessageInspector(string systemName)
        {
            this.systemName = systemName;
        }

        /// <summary>
        /// Gets called after receiving each soap request.
        /// </summary>
        /// <param name="request">The request message.</param>
        /// <param name="channel">The incoming channel.</param>
        /// <param name="instanceContext">The current service instance.</param>
        /// <returns>
        /// The object used to correlate state. This object is passed back in the <see cref="M:System.ServiceModel.Dispatcher.IDispatchMessageInspector.BeforeSendReply(System.ServiceModel.Channels.Message@,System.Object)"/> method.
        /// </returns>
        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
        {
            ServiceMessageLog log = null;

            try
            {
                var traceWriter = OperationContextAppender.AddTraceWriter();

                // add trace writer if service trace logging is enabled
                //if (this.enableTraceLogging)
                //{
                //    var traceWriter = OperationContextAppender.AddTraceWriter();
                //}

                log = this.CreateServiceLog(ref request);

                log.Id = Guid.NewGuid();

                log.ContextId = Guid.NewGuid();

                this.LogRequestInDatabase(ref log);

                //if (!request.Properties.ContainsKey(LogConstants.ServiceLogId))
                //{
                //    request.Properties.Add(LogConstants.ServiceLogId, log.Id);
                //}

                //if (!request.Properties.ContainsKey(DataContracts.Constants.ContextIdHeaderName))
                //{
                //    request.Properties.Add(DataContracts.Constants.ContextIdHeaderName, log.ContextId.GetValueOrDefault());
                //}

                //if (!request.Properties.ContainsKey(DataContracts.Constants.UserNameHeaderName) && !string.IsNullOrEmpty(log.UserName))
                //{
                //    request.Properties.Add(DataContracts.Constants.UserNameHeaderName, log.UserName);
                //}

                //if (OperationContext.Current != null &&
                //    OperationContext.Current.InstanceContext != null &&
                //    OperationContext.Current.InstanceContext.State != CommunicationState.Closed &&
                //    OperationContext.Current.IncomingMessageProperties != null)
                //{
                //    if (!OperationContext.Current.IncomingMessageProperties.ContainsKey(LogConstants.ServiceLogId))
                //    {
                //        OperationContext.Current.IncomingMessageProperties.Add(LogConstants.ServiceLogId, log.Id);
                //    }

                //    if (!OperationContext.Current.IncomingMessageProperties.ContainsKey(DataContracts.Constants.ContextIdHeaderName))
                //    {
                //        OperationContext.Current.IncomingMessageProperties.Add(DataContracts.Constants.ContextIdHeaderName, log.ContextId.GetValueOrDefault());
                //    }

                //    if (!OperationContext.Current.IncomingMessageProperties.ContainsKey(DataContracts.Constants.UserNameHeaderName) && !string.IsNullOrEmpty(log.UserName))
                //    {
                //        OperationContext.Current.IncomingMessageProperties.Add(DataContracts.Constants.UserNameHeaderName, log.UserName);
                //    }
                //}
            }
            catch (Exception ex1)
            {
                try
                {
                    // Log Exception

                    // Auto fallback write log to file system
                    //this.LogRequestInFileSystem(ref log);
                }
                catch (Exception ex2)
                {
                    // Log Exception
                }
            }

            return log;
        }

        /// <summary>
        /// Gets called before sending the soap response back to client
        /// </summary>
        /// <param name="reply">The reply message. This value is null if the operation is one way.</param>
        /// <param name="correlationState">The correlation object returned from the <see cref="M:System.ServiceModel.Dispatcher.IDispatchMessageInspector.AfterReceiveRequest(System.ServiceModel.Channels.Message@,System.ServiceModel.IClientChannel,System.ServiceModel.InstanceContext)"/> method.</param>
        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            string trace = string.Empty;

            var traceWriter = OperationContextAppender.RemoveTraceWriter();
            if (traceWriter != null)
            {
                trace = traceWriter.ToString();
                traceWriter.Dispose();
            }

            if (correlationState != null)
            {
                ServiceMessageLog log = correlationState as ServiceMessageLog;

                log.ServiceTrace = trace;
                log.ResponseXml = reply.ToString();
                log.ResponseCreatedOn = DateTime.Now;

                if (log != null)
                {
                    try
                    {
                        this.LogResponseInDatabase(log, true);
                    }
                    catch (Exception ex1)
                    {
                    }
                }
            }
        }

        #region Log in Database
        /// <summary>
        /// Logs the request in database.
        /// </summary>
        /// <param name="log">The log.</param>
        private void LogRequestInDatabase(ref ServiceMessageLog log)
        {
            ILogPersister logPersister = new AzureLogPersister();

            logPersister.Save(log);
        }

        /// <summary>
        /// Logs the request in database.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="contextId">The context id.</param>
        /// <param name="bookingLogs">The booking logs.</param>
        /// <param name="isFault">if set to <c>true</c> [is fault].</param>
        /// <param name="trace">The trace.</param>
        /// <param name="hasError">The has error.</param>
        /// <param name="hasWarning">The has warning.</param>
        private void LogResponseInDatabase(ServiceMessageLog log, bool updateOperation = false)
        {
            ILogPersister logPersister = new AzureLogPersister();

            logPersister.Save(log, updateOperation);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Creates the service log object from message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>returns service log instance</returns>
        private ServiceMessageLog CreateServiceLog(ref System.ServiceModel.Channels.Message request)
        {
            string message = string.Empty;
            var log = new ServiceMessageLog();
            log.SystemName = this.systemName;
            log.ServiceName = request.Headers.GetServiceName();
            log.ServiceMethod = request.Headers.GetMethodName();
            log.ServicePath = request.Headers.Action;
            log.RequestCreatedOn = DateTime.Now;
            log.IsFault = false;

            message = request.ToString();

            if (!string.IsNullOrEmpty(message))
            {
                log.RequestXml = message;
            }

            try
            {
                ////Could get security exception  
                log.HostName = Environment.MachineName;

                // Only update username if its specified in the context
                if (string.IsNullOrEmpty(log.UserName))
                {
                    log.UserName = Environment.UserName;
                }
            }
            catch (Exception)
            {
                ////Ignore all exceptions
            }

            return log;
        }

        /// <summary>
        /// Formats the XML string.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>
        /// returns string
        /// </returns>
        private string FormatXmlString(string xml)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                xml = xml.Replace(Environment.NewLine, string.Empty);
            }

            return xml;
        }

        #endregion
    }
}
