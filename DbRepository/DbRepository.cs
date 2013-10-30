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
            var command = GetCommand(procedure, parameters);
            var result = default(object);
            result = _db.ExecuteScalar(command);
            return (T)result;
        }

        public IEnumerable<T> Read<T>(string procedure, IDictionary<string, object> parameters) where T : new()
        {
            var mapper = MapBuilder<T>.BuildAllProperties();
            var command = GetCommand(procedure, parameters);
            using (var reader = _db.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    yield return mapper.MapRow(reader);
                }
            }
        }

        public Task<T> GetAsync<T>(string procedure, IDictionary<string, object> parameters)
        {
            if (_db.SupportsAsync)
                return DoGetAsync<T>(procedure, parameters);
            return Task.FromResult(Get<T>(procedure, parameters));
        }

        public Task<IEnumerable<T>> ReadAsync<T>(string procedure, IDictionary<string, object> parameters) where T : new()
        {
            if (_db.SupportsAsync)
                return DoReadAsync<T>(procedure, parameters);
            return Task.FromResult(Read<T>(procedure, parameters));
        }

        #region Privates
        private DbCommand GetCommand(string procedure, IDictionary<string, object> parameters)
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

        private Task PrepareCommand(DbCommand command, DbConnection connection)
        {
            if (command.Connection == default(DbConnection))
                command.Connection = connection;
            return connection.OpenAsync();
        }
        #endregion

        #region Internals
        private async Task<T> DoGetAsync<T>(string procedure, IDictionary<string, object> parameters)
        {
            using (var connection = _db.CreateConnection())
            {
                var command = GetCommand(procedure, parameters);
                await PrepareCommand(command, connection);
                var result = default(object);
                result = await command.ExecuteScalarAsync();
                return (T)result;
            }
        }

        private async Task<IEnumerable<T>> DoReadAsync<T>(string procedure, IDictionary<string, object> parameters) where T : new()
        {
            var mapper = MapBuilder<T>.BuildAllProperties();
            var result = new List<T>();
            using (var connection = _db.CreateConnection())
            {
                var command = GetCommand(procedure, parameters);
                await PrepareCommand(command, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(mapper.MapRow(reader));
                    }
                }
            }
            return result;
        } 
        #endregion
    }
}
