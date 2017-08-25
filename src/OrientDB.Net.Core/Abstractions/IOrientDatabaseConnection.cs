using System;
using OrientDB.Net.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrientDB.Net.Core.Abstractions
{
    public interface IOrientDatabaseConnection : IDisposable
    {
        Task<IEnumerable<TResultType>> ExecuteQueryAsync<TResultType>(string sql) where TResultType : OrientDBEntity;
        IEnumerable<TResultType> ExecuteQuery<TResultType>(string sql) where TResultType : OrientDBEntity;
        IEnumerable<TResultType> ExecutePreparedQuery<TResultType>(string sql, params string[] parameters) where TResultType : OrientDBEntity;
        IOrientDBCommandResult ExecuteCommand(string sql);
        IOrientDBTransaction CreateTransaction();
        Task<IOrientDBTransaction> CreateTransactionAsync();
    }
}
