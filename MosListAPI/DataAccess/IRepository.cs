using System.Collections.Generic;
using System.Threading.Tasks;

namespace MosListAPI.DataAccess{
    public interface IRepository<T>{
        Task<T> CreateAsync(T item);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetAsync(string id);
    }
}