namespace Framework.Services.Storage
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Configuration;
    /// <summary>
    /// The table service.
    /// </summary>
    public class TableService : ITableService
    {
        #region Constants

        private const string StorageConnectionString = "Framework.Services.Storage.StorageConnectionString";

        #endregion Constants

        #region Private members

        CloudStorageAccount storageAccount;
        CloudTableClient tableClient;

        #endregion Private members

        #region Constructor(s)

        /// <summary>
        /// Intializes the table service.
        /// </summary>
        public TableService()
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings[StorageConnectionString]);
            tableClient = storageAccount.CreateCloudTableClient();
        }

        public TableService(string storageConnectionString)
        {
            storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            tableClient = storageAccount.CreateCloudTableClient();
        }
        #endregion  Constructor(s)

        #region Public methods

        /// <summary>
        /// Creates table if it does not exists already.
        /// </summary>
        /// <param name="cloudTable"></param>
        /// <returns></returns>
        public async Task<bool> CreateTableAsync(CloudTable cloudTable)
        {
            EnsureValidArguments(cloudTable);
            // Create the CloudTable if it does not exist
            return await cloudTable.CreateIfNotExistsAsync();
        }

        /// <summary>
        /// Creates table if it does not exists already.
        /// </summary>
        /// <param name="cloudTable"></param>
        /// <returns></returns>
        public async Task<bool> CreateTableAsync(string cloudTableName)
        {
            EnsureValidArguments(cloudTableName);
            CloudTable cloudTable = tableClient.GetTableReference(cloudTableName);
            return await CreateTableAsync(cloudTable);
        }

        /// <summary>
        /// Inserts an entity to the table.
        /// </summary>
        /// <param name="cloudTable"></param>
        /// <param name="tableEntity"></param>
        /// <returns></returns>
        public async Task<TableResult> InsertEntityAsync(CloudTable cloudTable, TableEntity tableEntity)
        {
            EnsureValidArguments(cloudTable, tableEntity);
            // Create TableOperation that inserts the two factor request entity.
            TableOperation insertOperation = TableOperation.Insert(tableEntity);

            return await cloudTable.ExecuteAsync(insertOperation);
        }

        /// <summary>
        /// Updates an entity to the table.
        /// </summary>
        /// <param name="cloudTable"></param>
        /// <param name="tableEntity"></param>
        /// <returns></returns>
        public async Task<TableResult> UpdateEntityAsync(CloudTable cloudTable, TableEntity tableEntity)
        {
            EnsureValidArguments(cloudTable, tableEntity);
            // Create TableOperation that updates the two factor request entity.
            TableOperation updateOperation = TableOperation.Replace(tableEntity);

            return await cloudTable.ExecuteAsync(updateOperation);
        }

        /// <summary>
        /// Inserts an entity to the table.
        /// </summary>
        /// <param name="cloudTableName"></param>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        public async Task<TableResult> InsertEntityAsync(string cloudTableName, TableEntity tableEntity)
        {
            EnsureValidArguments(cloudTableName);
            CloudTable cloudTable = tableClient.GetTableReference(cloudTableName);

            return await InsertEntityAsync(cloudTable, tableEntity);
        }

        /// <summary>
        /// Updates an entity to the table.
        /// </summary>
        /// <param name="cloudTableName"></param>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        public async Task<TableResult> UpdateEntityAsync(string cloudTableName, TableEntity tableEntity)
        {
            EnsureValidArguments(cloudTableName);
            CloudTable cloudTable = tableClient.GetTableReference(cloudTableName);

            return await UpdateEntityAsync(cloudTable, tableEntity);
        }


        /// <summary>
        /// Returns the list of all the entities in the partition.
        /// </summary>
        /// <param name="cloudTable"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public async Task<List<TableEntity>> GetAllPartitionEntitiesAsync(CloudTable cloudTable, string partitionKey)
        {
            EnsureValidArguments(cloudTable, partitionKey);
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<TableEntity> query = new TableQuery<TableEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

            var result = new List<TableEntity>();
            // Print the fields for each customer.
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<TableEntity> resultSegment = await cloudTable.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                foreach (TableEntity entity in resultSegment.Results)
                {
                    result.Add(entity);
                }
            } while (token != null);

            return await Task.Run(() => result);
        }

        /// <summary>
        /// Returns the list of all the entities in the partition.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cloudTable"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public async Task<List<T>> GetAllPartitionEntitiesAsync<T>(CloudTable cloudTable, string partitionKey) where T : TableEntity, new()
        {
            EnsureValidArguments(cloudTable, partitionKey);
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<T> query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

            var result = new List<T>();
            // Print the fields for each customer.
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<T> resultSegment = await cloudTable.ExecuteQuerySegmentedAsync<T>(query, token);
                token = resultSegment.ContinuationToken;

                foreach (T entity in resultSegment.Results)
                {
                    result.Add(entity);
                }
            } while (token != null);

            return await Task.Run(() => result);
        }

        /// <summary>
        /// Returns the list of all the entities in the partition.
        /// </summary>
        /// <param name="cloudTableName"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public async Task<List<TableEntity>> GetAllPartitionEntitiesAsync(string cloudTableName, string partitionKey)
        {
            EnsureValidArguments(cloudTableName);
            // Getting cloud table.
            CloudTable cloudTable = tableClient.GetTableReference(cloudTableName);
            return await GetAllPartitionEntitiesAsync(cloudTable, partitionKey);
        }

        /// <summary>
        /// Returns the list of all the entities in the partition.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cloudTableName"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public async Task<List<T>> GetAllPartitionEntitiesAsync<T>(string cloudTableName, string partitionKey) where T : TableEntity, new()
        {
            EnsureValidArguments(cloudTableName);
            // Getting cloud table.
            CloudTable cloudTable = tableClient.GetTableReference(cloudTableName);
            return await GetAllPartitionEntitiesAsync<T>(cloudTable, partitionKey);
        }

        /// <summary>
        /// Returns single entity based on partition and row id.
        /// </summary>
        /// <param name="cloudTable"></param>
        /// <param name="partitionKey"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        public async Task<TableEntity> GetSingleEntityAsync(CloudTable cloudTable, string partitionKey, string rowId)
        {
            EnsureValidArguments(cloudTable, partitionKey, rowId);
            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<TableEntity>(partitionKey, rowId);

            // Execute the retrieve operation.
            TableResult retrievedResult = await cloudTable.ExecuteAsync(retrieveOperation);
            return await Task.Run(() => (TableEntity)retrievedResult.Result);
        }

        /// <summary>
        /// Returns single entity based on partition and row id.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cloudTable"></param>
        /// <param name="partitionKey"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        public async Task<T> GetSingleEntityAsync<T>(CloudTable cloudTable, string partitionKey, string rowId) where T : TableEntity, new()
        {
            EnsureValidArguments(cloudTable, partitionKey, rowId);
            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowId);

            // Execute the retrieve operation.
            TableResult retrievedResult = await cloudTable.ExecuteAsync(retrieveOperation);
            return await Task.Run(() => (T)retrievedResult.Result);
        }

        /// <summary>
        /// Returns single entity based on partition and row id.
        /// </summary>
        /// <param name="cloudTableName"></param>
        /// <param name="partitionKey"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        public async Task<TableEntity> GetSingleEntityAsync(string cloudTableName, string partitionKey, string rowId)
        {
            EnsureValidArguments(cloudTableName);
            // Getting cloud table.
            CloudTable cloudTable = tableClient.GetTableReference(cloudTableName);
            return await GetSingleEntityAsync(cloudTable, partitionKey, rowId);
        }

        /// <summary>
        /// Returns single entity based on partition and row id.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cloudTableName"></param>
        /// <param name="partitionKey"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        public async Task<T> GetSingleEntityAsync<T>(string cloudTableName, string partitionKey, string rowId) where T : TableEntity, new()
        {
            EnsureValidArguments(cloudTableName);
            // Getting cloud table.
            CloudTable cloudTable = tableClient.GetTableReference(cloudTableName);
            return await GetSingleEntityAsync<T>(cloudTable, partitionKey, rowId);
        }

        /// <summary>
        /// Deletes an entity async.
        /// </summary>
        /// <param name="cloudTable"></param>
        /// <param name="partitionKey"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        public async Task<TableResult> DeleteEntityAsync(CloudTable cloudTable, string partitionKey, string rowId)
        {
            EnsureValidArguments(cloudTable, partitionKey, rowId);
            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<TableEntity>(partitionKey, rowId);

            // Execute the retrieve operation.
            var retrievedResult = await cloudTable.ExecuteAsync(retrieveOperation);
            var retrievedEntity = (TableEntity)retrievedResult.Result;
            // Create the Delete TableOperation and then execute it.
            if (retrievedEntity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(retrievedEntity);

                // Execute the operation.
                return await cloudTable.ExecuteAsync(deleteOperation);
            }

            return null;

        }

        /// <summary>
        /// Deletes an entity async.
        /// </summary>
        /// <param name="cloudTable"></param>
        /// <param name="partitionKey"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        public async Task<TableResult> DeleteEntityAsync(string cloudTableName, string partitionKey, string rowId)
        {
            EnsureValidArguments(cloudTableName);
            // Getting cloud table.
            CloudTable cloudTable = tableClient.GetTableReference(cloudTableName);
            return await DeleteEntityAsync(cloudTable, partitionKey, rowId);
        }

        #endregion Public methods

        #region Private methods

        private void EnsureValidArguments(string cloudTableName)
        {
            if (string.IsNullOrEmpty(cloudTableName))
            {
                throw new ArgumentNullException("cloudTable");
            }
        }

        private void EnsureValidArguments(CloudTable cloudTable)
        {
            if(cloudTable == null)
            {
                throw new ArgumentNullException("cloudTable");
            }
        }
        private void EnsureValidArguments(CloudTable cloudTable, TableEntity tableEntity)
        {
            EnsureValidArguments(cloudTable);

            if (tableEntity == null)
            {
                throw new ArgumentNullException("tableEntity");
            }
        }

        private void EnsureValidArguments(CloudTable cloudTable, string partitionKey)
        {
            EnsureValidArguments(cloudTable);

            if (string.IsNullOrEmpty(partitionKey))
            {
                throw new ArgumentNullException("partitionKey");
            }
        }

        private void EnsureValidArguments(CloudTable cloudTable, string partitionKey, string rowId)
        {
            EnsureValidArguments(cloudTable, partitionKey);

            if (string.IsNullOrEmpty(rowId))
            {
                throw new ArgumentNullException("rowId");
            }
        }

        #endregion Private methods
    }
}
