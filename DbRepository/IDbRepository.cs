using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbRepository
{
    public interface IDbRepository
    {
        T Get<T>(string procedure, IDictionary<string, object> parameters);
        Task<T> GetAsync<T>(string procedure, IDictionary<string, object> parameters);
        IEnumerable<T> Read<T>(string procedure, IDictionary<string, object> parameters) where T : new();
        Task<IEnumerable<T>> ReadAsync<T>(string procedure, IDictionary<string, object> parameters) where T : new();
    }
}
