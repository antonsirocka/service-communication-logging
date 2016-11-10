using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace LogsDataAccess.AzureImplementation
{
    /// <summary>
    /// Azure entity for Log table
    /// </summary>
    public class LogEntity : TableEntity
    {
        /// <summary>
        /// Initialises a new instance of LogEntity
        /// </summary>
        public LogEntity()
        {
        }

        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The context Id
        /// </summary>
        public Guid? ContextId { get; set; }

        /// <summary>
        /// Gets or sets the ServiceName
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the MethodName
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets the ServicePath
        /// </summary>
        public string ServicePath { get; set; }

        /// <summary>
        /// Gets or sets the HostName
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets the RequestDate
        /// </summary>
        public DateTime? RequestDate { get; set; }

        /// <summary>
        /// Gets or sets the ResponseDate
        /// </summary>
        public DateTime? ResponseDate { get; set; }

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
    }
}
