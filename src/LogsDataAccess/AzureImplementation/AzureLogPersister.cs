using LogsDataAccess.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogsDataAccess.AzureImplementation
{
    /// <summary>
    /// AzureLogPersister implementation of ILogPersister interface
    /// </summary>
    public class AzureLogPersister : ILogPersister
    {
        /// <summary>
        /// The Azure storage account reference
        /// </summary>
        private readonly CloudStorageAccount storageAccount;

        /// <summary>
        /// Initialises a new instance of AzureLogPersister
        /// </summary>
        public AzureLogPersister()
        {
            storageAccount = CloudStorageAccount.Parse(
                "DefaultEndpointsProtocol=https;AccountName=servicecommunicationlogs;AccountKey=");
        }

        /// <summary>
        /// Gets a list of service message logs based on provided search term
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns>A list of service message logs</returns>
        public async Task<List<ServiceMessageLog>> Get(string searchTerm)
        {
            var tableClient = storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference("Logs");

            await table.CreateIfNotExistsAsync();

            TableQuery<LogEntity> query = new TableQuery<LogEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, searchTerm));

            var serviceMessageLogs = new List<ServiceMessageLog>();

            TableContinuationToken continuationToken = null;
            do
            {
                // Execute the query async until there is no more result
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                foreach (var entity in queryResult)
                {
                    serviceMessageLogs.Add(new ServiceMessageLog()
                    {
                        Id = entity.Id,
                        RequestXml = entity.RequestXml,
                        ResponseXml = entity.ResponseXml,
                        ServiceTrace = entity.ServiceTrace
                    });
                }

                continuationToken = queryResult.ContinuationToken;
            } while (continuationToken != null);

            return serviceMessageLogs;
        }

        /// <summary>
        /// Saves a specified service message log in Azure storage
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task Save(ServiceMessageLog log, bool updateOperation = false)
        {
            var tableClient = storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference("Logs");

            await table.CreateIfNotExistsAsync();

            // create log entry that contains both request and response
            // partition key is date
            var logEntry = new LogEntity()
            {
                PartitionKey = DateTime.Now.ToString("yyyyMMdd"),
                RowKey = log.Id.ToString(),
                Id = log.Id,
                ContextId = log.ContextId,
                ServiceTrace = log.ServiceTrace,
                RequestXml = log.RequestXml,
                ResponseXml = log.ResponseXml,
                ServiceName = log.ServiceName,
                MethodName = log.ServiceMethod,
                ServicePath = log.ServicePath,
                RequestDate = log.RequestCreatedOn,
                ResponseDate = log.ResponseCreatedOn,
                HostName = log.HostName
            };

            if (updateOperation)
            {
                logEntry.ETag = "*";
            }

            await table.ExecuteAsync(TableOperation.InsertOrReplace(logEntry));

            // create additional entry where partition key is the context id
            if (log.ContextId.HasValue)
            {
                logEntry.PartitionKey = log.ContextId.Value.ToString();
                
                if (!string.IsNullOrWhiteSpace(logEntry.RequestXml))
                {
                    logEntry.RequestXml = "see main entity";
                }

                if (!string.IsNullOrWhiteSpace(logEntry.ResponseXml))
                {
                    logEntry.ResponseXml = "see main entity";
                }

                await table.ExecuteAsync(TableOperation.InsertOrReplace(logEntry));
            }
        }
    }
}
