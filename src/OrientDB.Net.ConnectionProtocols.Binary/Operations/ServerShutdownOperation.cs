using OrientDB.Net.ConnectionProtocols.Binary.Contracts;
using System;
using OrientDB.Net.ConnectionProtocols.Binary.Core;
using System.IO;
using OrientDB.Net.ConnectionProtocols.Binary.Constants;
using OrientDB.Net.ConnectionProtocols.Binary.Extensions;
using OrientDB.Net.ConnectionProtocols.Binary.Operations.Results;
using OrientDB.Net.ConnectionProtocols.Binary.Operations;

internal class ServerShutdownOperation : IOrientDBOperation<VoidResult>
{
    private readonly ConnectionMetaData _connectionMetaData;
    private readonly string _username;
    private readonly string _password;

    public ServerShutdownOperation(ConnectionMetaData connectionMetaData, string username, string password)
    {
        this._connectionMetaData = connectionMetaData;
        this._username = username;
        this._password = password;
    }

    public Request CreateRequest(int sessionId, byte[] token)
    {
        Request request = new Request(OperationMode.Synchronous);

        // standard request fields
        request.AddDataItem((byte)OperationType.SHUTDOWN);
        request.AddDataItem(request.SessionId);

        request.AddDataItem(_username);
        request.AddDataItem(_password);

        return request;
    }

    public VoidResult Execute(BinaryReader reader)
    {
        return new VoidResult();
    }
}