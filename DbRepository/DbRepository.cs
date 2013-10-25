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
            var result = default(object);
            var command = PrepareCommand(procedure, parameters);
            using (var connection = _db.CreateConnection())
            {
                PrepareConnection(command, connection);
                result = command.ExecuteScalar();
            }
            return (T)result;
        }

        private DbCommand PrepareCommand(string procedure, IDictionary<string, object> parameters)
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

        private async Task PrepareConnectionAsync(DbCommand command, DbConnection connection)
        {
            if (command.Connection == default(DbConnection))
                command.Connection = connection;
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();
        }


        public async Task<T> GetAsync<T>(string procedure, IDictionary<string, object> parameters)
        {
            var result = default(object);
            var command = PrepareCommand(procedure, parameters);
            using (var connection = _db.CreateConnection())
            {
                if (_db.SupportsAsync)
                {
                    await PrepareConnectionAsync(command, connection);
                    result = await command.ExecuteScalarAsync();
                }
                else
                {
                    PrepareConnection(command, connection);
                    result = command.ExecuteScalar();
                }
            }
            return (T)result;
        }
    }
}
