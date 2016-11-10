using log4net.Appender;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace SampleService.Logging
{
    public class OperationContextAppender : DebugAppender
    {
        #region TraceWriter

        /// <summary>
        /// Gets the trace writer if it has been added by LogMessageInspector.
        /// </summary>
        /// <returns>
        /// returns string writer
        /// </returns>
        public static StringWriter GetTraceWriter()
        {
            StringWriter writer = null;
            if (OperationContext.Current != null &&
                OperationContext.Current.RequestContext != null &&
                OperationContext.Current.RequestContext.RequestMessage != null &&
                OperationContext.Current.RequestContext.RequestMessage.State != MessageState.Closed)
            {
                if (OperationContext.Current.OutgoingMessageProperties.ContainsKey("ServiceTraceWriter"))
                {
                    writer = OperationContext.Current.OutgoingMessageProperties["ServiceTraceWriter"] as StringWriter;
                }
            }

            return writer;
        }

        /// <summary>
        /// Adds the trace writer into operation context so trace can be written.
        /// </summary>
        /// <returns>
        /// returns string writer
        /// </returns>
        public static StringWriter AddTraceWriter()
        {
            StringWriter writer = null;
            if (OperationContext.Current != null &&
                OperationContext.Current.RequestContext != null &&
                OperationContext.Current.RequestContext.RequestMessage != null &&
                OperationContext.Current.RequestContext.RequestMessage.State != MessageState.Closed)
            {
                if (!OperationContext.Current.OutgoingMessageProperties.ContainsKey("ServiceTraceWriter"))
                {
                    writer = new StringWriter();
                    OperationContext.Current.OutgoingMessageProperties.Add("ServiceTraceWriter", writer);
                }
                else
                {
                    writer = OperationContext.Current.OutgoingMessageProperties["ServiceTraceWriter"] as StringWriter;
                }
            }

            return writer;
        }

        /// <summary>
        /// Removes the writer.
        /// </summary>
        /// <returns>
        /// returns string writer
        /// </returns>
        public static StringWriter RemoveTraceWriter()
        {
            StringWriter writer = null;
            if (OperationContext.Current != null &&
                OperationContext.Current.RequestContext != null &&
                OperationContext.Current.RequestContext.RequestMessage != null &&
                OperationContext.Current.RequestContext.RequestMessage.State != MessageState.Closed &&
                OperationContext.Current.OutgoingMessageProperties.ContainsKey("ServiceTraceWriter"))
            {
                writer = OperationContext.Current.OutgoingMessageProperties["ServiceTraceWriter"] as StringWriter;
                OperationContext.Current.OutgoingMessageProperties.Remove("ServiceTraceWriter");
            }

            return writer;
        }

        #endregion

        /// <summary>
        /// Appends the specified logging event.
        /// </summary>
        /// <param name="loggingEvent">The logging event.</param>
        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            this.AppendInternal(loggingEvent);
        }

        /// <summary>
        /// Appends the specified logging events.
        /// </summary>
        /// <param name="loggingEvents">The logging events.</param>
        protected override void Append(log4net.Core.LoggingEvent[] loggingEvents)
        {
            foreach (var loggingEvent in loggingEvents)
            {
                this.AppendInternal(loggingEvent);
            }
        }

        /// <summary>
        /// Appends the specified logging event.
        /// </summary>
        /// <param name="loggingEvents">The logging events.</param>
        protected void AppendInternal(params log4net.Core.LoggingEvent[] loggingEvents)
        {
            if (loggingEvents != null)
            {
                try
                {
                    StringWriter writer = GetTraceWriter();
                    if (writer != null)
                    {
                        foreach (var loggingEvent in loggingEvents)
                        {
                            ////loggingEvent.WriteRenderedMessage(writer);
                            this.RenderLoggingEvent(writer, loggingEvent);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var message = string.Format("Unable to write log to operation context appender.{0}Inner Exception:-{0}{0}{1}", Environment.NewLine, ex.ToString());
                }
            }
        }
    }
}
