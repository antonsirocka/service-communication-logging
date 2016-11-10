using LogStorage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace LogStorage.AzureImplementation
{
    public class AzureLogPersister : ILogPersister
    {
        public IList<ServiceMessageLog> Get()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                "DefaultEndpointsProtocol=https;AccountName=servicecommunicationlogs;AccountKey=xu8W9wf5ggsgjCO++5v5gFXZFcLG5sqFrBfbMU29uWapnt7v8L0lq72rIbdWjvuZsTQCK+9OmvOOe8nRHpMqcw==");

            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("Logs");
            table.CreateIfNotExists();

            TableQuery<LogEntity> query = new TableQuery<LogEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "20161006"));

            var entities = table.ExecuteQuery(query);

            //foreach (CustomerEntity entity in )
            //{
            //    Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
            //        entity.Email, entity.PhoneNumber);
            //}

            // Create the table query.
            //TableQuery<LogEntity> rangeQuery = new TableQuery<LogEntity>().Where(
            //    TableQuery.CombineFilters(
            //        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "20161006"),
            //        TableOperators.And,
            //        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, "E")));

            return null;
        }

        public void Save(ServiceMessageLog log)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                "DefaultEndpointsProtocol=https;AccountName=servicecommunicationlogs;AccountKey=xu8W9wf5ggsgjCO++5v5gFXZFcLG5sqFrBfbMU29uWapnt7v8L0lq72rIbdWjvuZsTQCK+9OmvOOe8nRHpMqcw==");

            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("Logs");
            table.CreateIfNotExists();

            var logEntry = new LogEntity()
            {
                PartitionKey = DateTime.Now.ToString("yyyyMMdd"),
                RowKey = log.ContextId.ToString() + "_" + log.Id.ToString(),
                ServiceTrace = log.ServiceTrace,
                RequestXml = log.RequestXml,
                ResponseXml = log.ResponseXml
            };

            var insertOperation = TableOperation.Insert(logEntry);
            table.Execute(insertOperation);
        }

        public void Update(ServiceMessageLog log)
        {

        }
    }
}
