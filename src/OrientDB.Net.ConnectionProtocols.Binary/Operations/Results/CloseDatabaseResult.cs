namespace OrientDB.Net.ConnectionProtocols.Binary.Operations.Results
{
    public class CloseDatabaseResult
    {
        public bool IsSuccess { get; }

        public CloseDatabaseResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }
    }
}
