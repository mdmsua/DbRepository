using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
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

        #region Implementations
        public T Scalar<T>(string procedure, Parameters parameters)
        {
            var command = GetCommand(procedure, parameters);
            var result = default(object);
            result = _db.ExecuteScalar(command);
            return (T)result;
        }

        public IReadOnlyCollection<T> Read<T>(string procedure, Parameters parameters) where T : new()
        {
            return new ReadOnlyCollection<T>(DoRead<T>(procedure, parameters).ToList());
        }

        public Task<T> ScalarAsync<T>(string procedure, Parameters parameters)
        {
            return ScalarAsync<T>(procedure, parameters, CancellationToken.None);
        }

        public Task<T> ScalarAsync<T>(string procedure, Parameters parameters, CancellationToken token)
        {
            if (_db.SupportsAsync)
                return DoGetAsync<T>(procedure, parameters, token);
            return Task.FromResult(Scalar<T>(procedure, parameters));
        }

        public Task<IReadOnlyCollection<T>> ReadAsync<T>(string procedure, Parameters parameters) where T : new()
        {
            return ReadAsync<T>(procedure, parameters, CancellationToken.None);
        }

        public Task<IReadOnlyCollection<T>> ReadAsync<T>(string procedure, Parameters parameters, CancellationToken token) where T : new()
        {
            if (_db.SupportsAsync)
                return DoReadAsync<T>(procedure, parameters, token);
            return Task.FromResult(Read<T>(procedure, parameters));
        }

        public bool Write(string procedure, Parameters parameters)
        {
            var rowsAffected = default(int);
            using (var wrapper = GetOpenConnection())
            {
                using (var transaction = wrapper.Connection.BeginTransaction())
                {
                    var command = GetCommand(procedure, parameters);
                    try
                    {
                        rowsAffected = _db.ExecuteNonQuery(command, transaction);
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return rowsAffected > default(int);
        }

        public bool Write(Procedures procedures)
        {
            var rowsAffected = new List<int>(procedures.Count);
            var commands = new List<DbCommand>(procedures.Count);
            using (var wrapper = GetOpenConnection())
            {
                using (var transaction = wrapper.Connection.BeginTransaction())
                {
                    foreach (var procedure in procedures)
                    {
                        commands.Add(GetCommand(procedure.Key, procedure.Value));
                    }
                    try
                    {
                        foreach (var command in commands)
                        {
                            rowsAffected.Add(_db.ExecuteNonQuery(command, transaction));
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return rowsAffected.All(r => r > 0);
        }

        public Task<bool> WriteAsync(string procedure, Parameters parameters)
        {
            return WriteAsync(procedure, parameters, CancellationToken.None);
        }

        public Task<bool> WriteAsync(string procedure, Parameters parameters, CancellationToken token)
        {
            if (_db.SupportsAsync)
                return DoWriteAsync(procedure, parameters, token);
            return Task.FromResult(Write(procedure, parameters));
        }

        public Task<bool> WriteAsync(Procedures procedures)
        {
            return WriteAsync(procedures, CancellationToken.None);
        }

        public Task<bool> WriteAsync(Procedures procedures, CancellationToken token)
        {
            if (_db.SupportsAsync)
                return DoWriteAsync(procedures, token);
            return Task.FromResult(Write(procedures));
        } 
        #endregion

        #region Privates
        private void SetParameter(DbCommand command, string name, object value)
        {
            var procParamName = _db.BuildParameterName(name);
            if (command.Parameters.Contains(procParamName))
            {
                DbType srcDbType, destDbType;
                destDbType = command.Parameters[procParamName].DbType;
                if (Enum.TryParse<DbType>(value.GetType().Name, out srcDbType) && srcDbType == destDbType)
                    _db.SetParameterValue(command, procParamName, value);
                else
                    throw new ParameterTypeException(command.CommandText, procParamName, destDbType.ToString(), srcDbType.ToString());
            }
        }

        private void SetParameters(DbCommand command, IReadOnlyDictionary<string, object> parameters)
        {
            parameters.ToList().ForEach(p => SetParameter(command, p.Key, p.Value));
        }

        private DbCommand GetCommand(string procedure, Parameters parameters)
        {
            var command = _db.GetStoredProcCommand(procedure);
            if (_db.SupportsParemeterDiscovery)
                _db.DiscoverParameters(command);
            SetParameters(command, parameters);
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

        private DatabaseConnectionWrapper GetOpenConnection()
        {
            DatabaseConnectionWrapper connection = TransactionScopeConnections.GetConnection(_db);
            return connection ?? GetWrappedConnection();
        }

        private DatabaseConnectionWrapper GetWrappedConnection()
        {
            return new DatabaseConnectionWrapper(GetNewOpenConnection());
        }

        private DbConnection GetNewOpenConnection()
        {
            DbConnection connection = null;
            try
            {
                connection = _db.CreateConnection();
                connection.Open();
            }
            catch
            {
                if (connection != null)
                    connection.Close();

                throw;
            }

            return connection;
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
        private IEnumerable<T> DoRead<T>(string procedure, Parameters parameters) where T : new()
        {
            var command = GetCommand(procedure, parameters);
            using (var reader = _db.ExecuteReader(command))
            {
                var mapper = MapBuilder<T>.BuildAllProperties();
                while (reader.Read())
                {
                    yield return mapper.MapRow(reader);
                }
            }
        }

        private async Task<T> DoGetAsync<T>(string procedure, Parameters parameters, CancellationToken token)
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

        private async Task<IReadOnlyCollection<T>> DoReadAsync<T>(string procedure, Parameters parameters, CancellationToken token) where T : new()
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

        private async Task<bool> DoWriteAsync(string procedure, Parameters parameters, CancellationToken token)
        {
            var result = default(bool);
            var rowsAffected = default(int);
            using (var wrapper = await GetOpenConnectionAsync())
            {
                using (var transaction = wrapper.Connection.BeginTransaction())
                {
                    var command = GetCommand(procedure, parameters);
                    PrepareCommand(command, transaction);
                    try
                    {
                        rowsAffected = await command.ExecuteNonQueryAsync(token);
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        result = rowsAffected > 0;
                    }
                }
            }
            return result;
        }

        private async Task<bool> DoWriteAsync(Procedures procedures, CancellationToken token)
        {
            var result = default(bool);
            var tasks = new List<Task<int>>(procedures.Count);
            using (var wrapper = await GetOpenConnectionAsync())
            {
                using (var transaction = wrapper.Connection.BeginTransaction())
                {
                    foreach (var procedure in procedures)
                    {
                        var command = GetCommand(procedure.Key, procedure.Value);
                        PrepareCommand(command, transaction);
                        tasks.Add(command.ExecuteNonQueryAsync(token));
                    }
                    try
                    {
                        await Task.WhenAll(tasks);
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        result = tasks.All(t => t.Result > 0);
                    }
                }
            }
            return result;
        }
        #endregion
    }
}
