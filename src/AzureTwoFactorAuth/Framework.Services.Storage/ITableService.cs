
namespace Framework.Services.Storage
{
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// ITableService
    /// </summary>
    public interface ITableService
    {
        /// <summary>
        /// Creates table if it does not exists already.
        /// </summary>
        /// <param name="cloudTable"></param>
        /// <returns></returns>
        Task<bool> CreateTableAsync(CloudTable cloudTable);

        /// <summary>
        /// Creates table if it does not exists already.
        /// </summary>
        /// <param name="requestedTable"></param>
        /// <returns></returns>
        Task<bool> CreateTableAsync(string cloudTableName);

        /// <summary>
        /// Inserts an entity to the table.
        /// </summary>
        /// <param name="cloudTable"></param>
        /// <param name="tableEntity"></param>
        /// <returns></returns>
        Task<TableResult> InsertEntityAsync(CloudTable cloudTable, TableEntity tableEntity);


        /// <summary>
        /// Updates an entity to the table.
        /// </summary>
        /// <param name="cloudTable"></param>
        /// <param name="tableEntity"></param>
        /// <returns></returns>
        Task<TableResult> UpdateEntityAsync(CloudTable cloudTable, TableEntity tableEntity);

        /// <summary>
        /// Inserts an entity to the table.
        /// </summary>
        /// <param name="cloudTableName"></param>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        Task<TableResult> InsertEntityAsync(string cloudTableName, TableEntity tableEntity);


        /// <summary>
        /// Updates an entity to the table.
        /// </summary>
        /// <param name="cloudTableName"></param>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        Task<TableResult> UpdateEntityAsync(string cloudTableName, TableEntity tableEntity);

        /// <summary>
        /// Returns the list of all the entities in the partition.
        /// </summary>
        /// <param name="cloudTable"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        Task<List<TableEntity>> GetAllPartitionEntitiesAsync(CloudTable cloudTable, string partitionKey);

        /// <summary>
        /// Returns the list of all the entities in the partition.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cloudTable"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        Task<List<T>> GetAllPartitionEntitiesAsync<T>(CloudTable cloudTable, string partitionKey) where T : TableEntity, new ();

        /// <summary>
        /// Returns the list of all the entities in the partition.
        /// </summary>
        /// <param name="cloudTable"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        Task<List<TableEntity>> GetAllPartitionEntitiesAsync(string cloudTableName, string partitionKey);

        /// <summary>
        /// Returns the list of all the entities in the partition.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cloudTableName"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        Task<List<T>> GetAllPartitionEntitiesAsync<T>(string cloudTableName, string partitionKey) where T : TableEntity, new();


        /// <summary>
        /// Returns single entity based on partition and row id.
        /// </summary>
        /// <param name="cloudTableName"></param>
        /// <param name="partitionKey"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        Task<TableEntity> GetSingleEntityAsync(CloudTable cloudTable, string partitionKey, string rowId);

        /// <summary>
        /// Returns single entity based on partition and row id.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cloudTableName"></param>
        /// <param name="partitionKey"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        Task<T> GetSingleEntityAsync<T>(CloudTable cloudTable, string partitionKey, string rowId) where T : TableEntity, new();

        /// <summary>
        /// Returns single entity based on partition and row id.
        /// </summary>
        /// <param name="cloudTableName"></param>
        /// <param name="partitionKey"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        Task<TableEntity> GetSingleEntityAsync(string cloudTableName, string partitionKey, string rowId);

        /// <summary>
        /// Returns single entity based on partition and row id.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cloudTableName"></param>
        /// <param name="partitionKey"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        Task<T> GetSingleEntityAsync<T>(string cloudTableName, string partitionKey, string rowId) where T : TableEntity, new();

        /// <summary>
        /// Deletes an entity async.
        /// </summary>
        /// <param name="cloudTable"></param>
        /// <param name="partitionKey"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        Task<TableResult> DeleteEntityAsync(CloudTable cloudTable, string partitionKey, string rowId);

        /// <summary>
        /// Deletes an entity async.
        /// </summary>
        /// <param name="cloudTableName"></param>
        /// <param name="partitionKey"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        Task<TableResult> DeleteEntityAsync(string cloudTableName, string partitionKey, string rowId);
    }
}
