using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogsDataAccess.Interfaces
{
    /// <summary>
    /// The LogPersister interface
    /// </summary>
    public interface ILogPersister
    {
        /// <summary>
        /// Gets a list of service message logs based on provided search term
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns>A list of service message logs</returns>
        Task<List<ServiceMessageLog>> Get(string searchTerm);

        /// <summary>
        /// Saves a specified service message log in Azure storage
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        Task Save(ServiceMessageLog log, bool updateOperation = false);
    }
}
