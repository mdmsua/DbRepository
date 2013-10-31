using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
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

        public Task<T> GetAsync<T>(string procedure, IDictionary<string, object> parameters, CancellationToken token)
        {
            if (_db.SupportsAsync)
                return DoGetAsync<T>(procedure, parameters, token);
            return Task.FromResult(Get<T>(procedure, parameters));
        }

        public Task<IEnumerable<T>> ReadAsync<T>(string procedure, IDictionary<string, object> parameters, CancellationToken token) where T : new()
        {
            if (_db.SupportsAsync)
                return DoReadAsync<T>(procedure, parameters, token);
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

        private void PrepareCommand(DbCommand command, DbConnection connection)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (connection == null) throw new ArgumentNullException("connection");
            
            command.Connection = connection;
        }

        private void PrepareCommand(DbCommand command, DbTransaction transaction)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (transaction == null) throw new ArgumentNullException("connection");

            PrepareCommand(command, transaction.Connection);
            command.Transaction = transaction;
        }

        private async Task<DatabaseConnectionWrapper> GetOpenConnectionAsync()
        {
            DatabaseConnectionWrapper connection = TransactionScopeConnections.GetConnection(_db);
            return connection ?? await GetWrappedConnectionAsync();
        }

        private async Task<DatabaseConnectionWrapper> GetWrappedConnectionAsync()
        {
            return new DatabaseConnectionWrapper(await GetNewOpenConnectionAsync());
        }

        private async Task<DbConnection> GetNewOpenConnectionAsync()
        {
            DbConnection connection = null;
            try
            {
                connection = _db.CreateConnection();
                await connection.OpenAsync();
            }
            catch
            {
                if (connection != null)
                    connection.Close();

                throw;
            }

            return connection;
        }
        #endregion

        #region Internals
        private async Task<T> DoGetAsync<T>(string procedure, IDictionary<string, object> parameters, CancellationToken token)
        {
            using (var wrapper = await GetOpenConnectionAsync())
            {
                var command = GetCommand(procedure, parameters);
                PrepareCommand(command, wrapper.Connection);
                var result = default(object);
                result = await command.ExecuteScalarAsync(token);
                return (T)result;
            }
        }

        private async Task<IEnumerable<T>> DoReadAsync<T>(string procedure, IDictionary<string, object> parameters, CancellationToken token) where T : new()
        {
            var mapper = MapBuilder<T>.BuildAllProperties();
            var result = new List<T>();
            using (var wrapper = await GetOpenConnectionAsync())
            {
                var command = GetCommand(procedure, parameters);
                PrepareCommand(command, wrapper.Connection);
                using (var reader = await command.ExecuteReaderAsync(token))
                {
                    while (await reader.ReadAsync(token))
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
