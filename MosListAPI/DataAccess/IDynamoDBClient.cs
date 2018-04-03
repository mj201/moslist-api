using System.Collections.Generic;
using System.Threading.Tasks;
namespace MosListAPI.DataAccess{
    public interface IDynamoDBClient{
        Task CreateTableAsync(string tableName, string idName = "Id", int readCapacityUnits = 1,
                                    int writeCapacityUnits = 1);

        Task<T> GetItemAsync<T>(string id);
        Task PutItemAsync<T>(T item);
        Task<IEnumerable<T>> ScanItemAsync<T>();
    }
}