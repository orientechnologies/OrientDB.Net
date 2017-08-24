namespace OrientDB.Net.SqlCommandBuilder.Interfaces
{
    public interface IOrientDBQueryable
    {
        IOrientDBQueryable Where(string field);

        IOrientDBQueryable And(string field);

        IOrientDBQueryable Or(string field);

        IOrientDBQueryable Equals<T>(T item);

        IOrientDBQueryable NotEquals<T>(T item);

        IOrientDBQueryable Lesser<T>(T item);

        IOrientDBQueryable LesserEqual<T>(T item);

        IOrientDBQueryable Greater<T>(T item);

        IOrientDBQueryable GreaterEqual<T>(T item);

        IOrientDBQueryable Like<T>(T item);

        IOrientDBQueryable IsNull();

        IOrientDBQueryable Contains<T>(T item);

        IOrientDBQueryable Contains<T>(string field, T value);

        IOrientDBQueryable Limit(int maxRecords);
    }
}
