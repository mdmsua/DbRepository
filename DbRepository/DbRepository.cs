using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace DbRepository
{
    public class DbRepository : IDbRepository
    {
        private readonly Database _db;

        static DbRepository()
        {
            DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory());
        }

        public DbRepository()
        {
            _db = DatabaseFactory.CreateDatabase();
        }

        public DbRepository(string name)
        {
            _db = DatabaseFactory.CreateDatabase(name);
        }

        public T Get<T>(string procedure, IDictionary<string, object> parameters)
        {
            var result = default(T);
            var command = GetDbCommand(procedure, parameters);
            result = (T)_db.ExecuteScalar(command);
            return result;
        }

        public Task<T> GetAsync<T>(string procedure, IDictionary<string, object> parameters)
        {
            if (_db.SupportsAsync)
                return GetAsyncInternal<T>(procedure, parameters);
            return Task.FromResult(Get<T>(procedure, parameters));
        }

        #region Privates
        private DbCommand GetDbCommand(string procedure, IDictionary<string, object> parameters)
        {
            var command = _db.GetStoredProcCommand(procedure);
            if (_db.SupportsParemeterDiscovery)
                _db.DiscoverParameters(command);
            foreach (var parameter in parameters)
            {
                _db.SetParameterValue(command, _db.BuildParameterName(parameter.Key), parameter.Value);
            }
            return command;
        }

        private void PrepareConnection(DbCommand command, DbConnection connection)
        {
            if (command.Connection == default(DbConnection))
                command.Connection = connection;
            if (connection.State != ConnectionState.Open)
                connection.Open();
        }

        private Task PrepareConnectionAsync(DbCommand command, DbConnection connection)
        {
            if (command.Connection == default(DbConnection))
                command.Connection = connection;
            if (connection.State != ConnectionState.Open)
                return connection.OpenAsync();
            return Task.Delay(0);
        }
        #endregion

        #region Internals
        private async Task<T> GetAsyncInternal<T>(string procedure, IDictionary<string, object> parameters)
        {
            var result = default(object);
            using (var connection = _db.CreateConnection())
            {
                var command = GetDbCommand(procedure, parameters);
                await PrepareConnectionAsync(command, connection);
                result = await command.ExecuteScalarAsync();
            }
            return (T)result;
        } 
        #endregion
    }
}
