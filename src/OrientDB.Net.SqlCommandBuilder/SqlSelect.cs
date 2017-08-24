using System.Collections.Generic;
using OrientDB.Net.SqlCommandBuilder.Protocol;
using OrientDB.Net.Core.Models;
using OrientDB.Net.Core.Exceptions;

// syntax:
// SELECT [FROM <Target> 
// [LET <Assignment>*](<Projections>]) 
// [<Condition>*](WHERE) 
// [BY <Field>](GROUP) 
// [BY <Fields>* [ASC|DESC](ORDER)*] 
// [<SkipRecords>](SKIP) 
// [<MaxRecords>](LIMIT)

namespace OrientDB.Net.SqlCommandBuilder
{
    public class SqlSelect
    {
        private SqlQuery _sqlQuery;

        public SqlSelect()
        {
            _sqlQuery = new SqlQuery();
        }

        #region Select

        public SqlSelect Select(params string[] projections)
        {
            _sqlQuery.Select(projections);

            return this;
        }

        public SqlSelect Also(string projection)
        {
            _sqlQuery.Also(projection);

            return this;
        }

        public SqlSelect Nth(int index)
        {
            _sqlQuery.Nth(index);

            return this;
        }

        public SqlSelect As(string alias)
        {
            _sqlQuery.As(alias);

            return this;
        }

        #endregion

        #region From

        public SqlSelect From(string target)
        {
            _sqlQuery.From(target);

            return this;
        }

        public SqlSelect From(ORID orid)
        {
            _sqlQuery.From(orid);

            return this;
        }

        public SqlSelect From(SqlSelect nestedSelect)
        {
            _sqlQuery.From(nestedSelect);
            return this;
        }

        public SqlSelect From(OrientDBEntity entity)
        {
            if ((entity.ORID == null) && string.IsNullOrEmpty(entity.OClassName))
            {
                throw new OrientDBException(OrientDBExceptionType.Query, "Document doesn't contain ORID or OClassName value.");
            }

            _sqlQuery.From(entity.OClassName);

            return this;
        }

        //public SqlSelect From<T>()
        //{
        //    //GRD: I think this would be the same thing?
        //    MemberInfo[] membersInfo = typeof(T).GetTypeInfo().GetProperties();
        //    foreach (MemberInfo memberInfo in membersInfo)
        //    {
        //        foreach (object attribute in memberInfo.GetCustomAttributes(typeof(OAliasAttribute), true))
        //        {
        //            return From(((OAliasAttribute)attribute).Name);
        //        }
        //    }

        //    return From(typeof(T).Name);
            

        //    //var tAttribute = (OAliasAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(OAliasAttribute), true);
        //    //if (tAttribute == null)
        //    //{
        //    //    return From(typeof(T).Name);
        //    //}
        //    //else
        //    //{
        //    //    return From(tAttribute.Name);
        //    //}
        //}

        #endregion

        #region Where with conditions

        public SqlSelect Where(params string[] fields)
        {
            _sqlQuery.Where(fields);

            return this;
        }

        public SqlSelect Where(IEnumerable<string> fields)
        {
            _sqlQuery.Where(fields);

            return this;
        }

        public SqlSelect And(string field)
        {
            _sqlQuery.And(field);

            return this;
        }

        public SqlSelect Or(string field)
        {
            _sqlQuery.Or(field);

            return this;
        }

        public SqlSelect Equals<T>(T item)
        {
            _sqlQuery.Equals<T>(item);

            return this;
        }

        public SqlSelect NotEquals<T>(T item)
        {
            _sqlQuery.NotEquals<T>(item);

            return this;
        }

        public SqlSelect Lesser<T>(T item)
        {
            _sqlQuery.Lesser<T>(item);

            return this;
        }

        public SqlSelect LesserEqual<T>(T item)
        {
            _sqlQuery.LesserEqual<T>(item);

            return this;
        }

        public SqlSelect Greater<T>(T item)
        {
            _sqlQuery.Greater<T>(item);

            return this;
        }

        public SqlSelect GreaterEqual<T>(T item)
        {
            _sqlQuery.GreaterEqual<T>(item);

            return this;
        }

        public SqlSelect Like<T>(T item)
        {
            _sqlQuery.Like<T>(item);

            return this;
        }

        public SqlSelect Lucene<T>(T item)
        {
            _sqlQuery.Lucene<T>(item);

            return this;
        }


        public SqlSelect IsNull()
        {
            _sqlQuery.IsNull();

            return this;
        }

        public SqlSelect Contains<T>(T item)
        {
            _sqlQuery.Contains<T>(item);

            return this;
        }

        public SqlSelect Contains<T>(string field, T value)
        {
            _sqlQuery.Contains<T>(field, value);

            return this;
        }

        public SqlSelect In<T>(IList<T> list)
        {
            _sqlQuery.In(list);

            return this;
        }

        public SqlSelect Between(int num1, int num2)
        {
            _sqlQuery.Between(num1, num2);
            return this;
        }
        #endregion

        public SqlSelect OrderBy(params string[] fields)
        {
            _sqlQuery.OrderBy(fields);

            return this;
        }

        public SqlSelect Ascending()
        {
            _sqlQuery.Ascending();

            return this;
        }

        public SqlSelect Descending()
        {
            _sqlQuery.Descending();

            return this;
        }

        public SqlSelect Skip(int skipCount)
        {
            _sqlQuery.Skip(skipCount);

            return this;
        }

        public SqlSelect Limit(int maxRecords)
        {
            _sqlQuery.Limit(maxRecords);

            return this;
        }      

        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.Select);
        }
    }
}
