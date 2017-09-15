using OrientDB.Net.Core.Abstractions;
using OrientDB.Net.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrientDB.Net.ConnectionProtocols.Binary.Contracts
{
    public interface IOrientDBCommand
    {
        IEnumerable<T> Execute<T>(string query) where T : OrientDBEntity;

        IEnumerable<T> ExecutePrepared<T>(string query, params string[] parameters) where T : OrientDBEntity;

        IOrientDBCommandResult Execute(string query);

        Task<IEnumerable<T>> ExecuteAsync<T>(string query) where T : OrientDBEntity;

        Task<IOrientDBCommandResult> ExecuteAsync(string query);
    }
}