using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Logging;
using MosListAPI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace MosListAPI.DataAccess{
    public class DynamoDBClient : IDynamoDBClient{
        #region " Members "
        private IAmazonDynamoDB _client;
        private DynamoDBContext _context;
        private ILogger _logger;
        private List<string> _tables;
        #endregion

        #region " Constructors "
        public DynamoDBClient(IAmazonDynamoDB client, ILogger<DynamoDBClient> logger){
            _client = client;
            _context = new DynamoDBContext(_client);
            _logger = logger;
            _tables = null;
        }
        #endregion

        #region " Methods "
        // Primarily for local development only. Table creation/provisioning for production needs to be
        // handled in provisioning script
        public async Task CreateTableAsync(string tableName, string idName = "Id", int readCapacityUnits = 1,
                                    int writeCapacityUnits = 1){
            // First check to see if the table already exists in the database
            if (await TableExists(tableName))
                return;

            // Create the table if it does not already exist
            var request = new CreateTableRequest{
                AttributeDefinitions = new List<AttributeDefinition>{
                    new AttributeDefinition{
                        AttributeName = idName,
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>{
                    new KeySchemaElement{
                        AttributeName = idName,
                        KeyType = "HASH"
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput{
                    ReadCapacityUnits = readCapacityUnits,
                    WriteCapacityUnits = writeCapacityUnits
                },
                TableName = tableName
            };

            var response = await _client.CreateTableAsync(request);

            var tableDescription = response.TableDescription;
            _logger.LogInformation("{1}: {0} \t ReadsPerSec: {2} \t WritesPerSec: {3}",
                      tableDescription.TableStatus,
                      tableDescription.TableName,
                      tableDescription.ProvisionedThroughput.ReadCapacityUnits,
                      tableDescription.ProvisionedThroughput.WriteCapacityUnits);

            string status = tableDescription.TableStatus;
            _logger.LogInformation(tableName + " - " + status);

            await WaitUntilTableReady(tableName);
        }
        public async Task<T> GetItemAsync<T>(string id){
            var item = await _context.LoadAsync<T>(id);
            return item;
        }
        private async Task<List<string>> GetTables()
        {
            if (_tables != null)
                return _tables;
            _tables = new List<string>();
            string lastTableNameEvaluated = null;
            do
            {
                var request = new ListTablesRequest
                {
                    Limit = 100,
                    ExclusiveStartTableName = lastTableNameEvaluated
                };

                var response = await _client.ListTablesAsync(request);
                foreach (string name in response.TableNames)
                    _tables.Add(name);
                lastTableNameEvaluated = response.LastEvaluatedTableName;
            } while (lastTableNameEvaluated != null);
            return _tables;
        }
        public async Task PutItemAsync<T>(T item){
            await _context.SaveAsync(item);
        }
        public async Task<IEnumerable<T>> ScanItemAsync<T>(){
            // TODO: change this to use paging. DynamoDB kind of sucks this way
            var searchResults = _context.ScanAsync<T>(null, null);
            List<T> returnSet = new List<T>();
            while (!searchResults.IsDone){
                returnSet.AddRange(await searchResults.GetNextSetAsync());
            }
            return returnSet;
        }
        public async Task<bool> TableExists(string tableName){
            var tables = await GetTables();
            return tables.Contains(tableName);
        }
        private async Task WaitUntilTableReady(string tableName)
        {
            string status = null;
            // Let us wait until table is created. Call DescribeTable.
            do
            {
                System.Threading.Thread.Sleep(5000); // Wait 5 seconds.
                try
                {
                    var res = await _client.DescribeTableAsync(new DescribeTableRequest
                    {
                        TableName = tableName
                    });

                    _logger.LogInformation("Table name: {0}, status: {1}",
                              res.Table.TableName,
                              res.Table.TableStatus);
                    status = res.Table.TableStatus;
                }
                catch (ResourceNotFoundException)
                {
                    // DescribeTable is eventually consistent. So you might
                    // get resource not found. So we handle the potential exception.
                }
            } while (status != "ACTIVE");
        }
        #endregion 
    }
}