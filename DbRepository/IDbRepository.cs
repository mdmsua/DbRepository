using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DbRepository
{
    public interface IDbRepository
    {
        T Scalar<T>(string procedure, Parameters parameters);
        Task<T> ScalarAsync<T>(string procedure, Parameters parameters);
        Task<T> ScalarAsync<T>(string procedure, Parameters parameters, CancellationToken token);

        IReadOnlyCollection<T> Read<T>(string procedure, Parameters parameters) where T : new();
        Task<IReadOnlyCollection<T>> ReadAsync<T>(string procedure, Parameters parameters) where T : new();
        Task<IReadOnlyCollection<T>> ReadAsync<T>(string procedure, Parameters parameters, CancellationToken token) where T : new();

        bool Write(Procedures procedures);
        bool Write(string procedure, Parameters parameters);
        Task<bool> WriteAsync(string procedure, Parameters parameters);
        Task<bool> WriteAsync(string procedure, Parameters parameters, CancellationToken token);
        Task<bool> WriteAsync(Procedures procedures);
        Task<bool> WriteAsync(Procedures procedures, CancellationToken token);
    }
}
