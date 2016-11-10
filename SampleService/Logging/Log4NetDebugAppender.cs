using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleService.Logging
{
    public class Log4NetDebugAppender : log4net.Appender.DebugAppender
    {
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
        /// <param name="loggingEvent">The logging event.</param>
        protected void AppendInternal(log4net.Core.LoggingEvent loggingEvent)
        {
            Debug.Write(this.RenderLoggingEvent(loggingEvent));
            if (this.ImmediateFlush)
            {
                Debug.Flush();
            }
        }
    }
}
