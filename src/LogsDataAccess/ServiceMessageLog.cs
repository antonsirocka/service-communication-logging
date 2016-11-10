using System;

namespace LogsDataAccess
{
    public class ServiceMessageLog
    {
        public virtual Guid Id { get; set; }
        
        /// <summary>
        /// System name
        /// </summary>
        public virtual string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the ServiceName
        /// </summary>
        public virtual string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the ServiceMethod
        /// </summary>
        public virtual string ServiceMethod { get; set; }

        /// <summary>
        /// Gets or sets the ServicePath
        /// </summary>
        public virtual string ServicePath { get; set; }

        /// <summary>
        /// Gets or sets the ContextId
        /// </summary>
        public virtual Guid? ContextId { get; set; }

        /// <summary>
        /// Gets or sets the Host
        /// </summary>
        public virtual string HostName { get; set; }

        /// <summary>
        /// Gets or sets the RequestId
        /// </summary>
        public virtual Guid? RequestId { get; set; }

        /// <summary>
        /// Gets or sets the UserId
        /// </summary>
        public virtual Guid? UserId { get; set; }

        /// <summary>
        /// Gets or sets the UserId
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        ///  Gets or sets the Request CreatedOn
        /// </summary>
        public virtual DateTime? RequestCreatedOn { get; set; }

        /// <summary>
        ///  Gets or sets the Response CreatedOn
        /// </summary>
        public virtual DateTime? ResponseCreatedOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fault.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is fault; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsFault { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has error; otherwise, <c>false</c>.
        /// </value>
        public virtual bool? HasError { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has warning.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has warning; otherwise, <c>false</c>.
        /// </value>
        public virtual bool? HasWarning { get; set; }

        /// <summary>
        /// Gets or sets the RequestXml
        /// </summary>
        public virtual string RequestXml { get; set; }

        /// <summary>
        /// Gets or sets the ResponseXml
        /// </summary>
        public virtual string ResponseXml { get; set; }

        /// <summary>
        /// Gets or sets the service trace.
        /// </summary>
        /// <value>
        /// The service trace.
        /// </value>
        public virtual string ServiceTrace { get; set; }

        /// <summary>
        /// Gets or sets the log sequence.
        /// </summary>
        /// <value>
        /// The log sequence.
        /// </value>
        public virtual int LogSequence { get; set; }
    }
}
