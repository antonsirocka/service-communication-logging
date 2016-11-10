using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogStorage.AzureImplementation
{
    public class LogEntity : TableEntity
    {
        public LogEntity()
        {
        }

        public Guid Id { get; set; }

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
