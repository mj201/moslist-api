using System.Collections.Generic;
using System.Threading.Tasks;
namespace MosListAPI.DataAccess{
    public interface IElasticSearchClient{
        Task<T> GetItemAsync<T>(string id);
        Task PutItemAsync<T>(T item);
    }
}