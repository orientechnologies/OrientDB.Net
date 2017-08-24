using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OrientDB.Net.SqlCommandBuilder.Queryable
{
    public class QueryableOrientDBData<OrientDBEntity> : IOrderedQueryable<OrientDBEntity>
    {
        public Expression Expression => Expression.Constant(this);

        public Type ElementType => typeof(OrientDBEntity);

        public IQueryProvider Provider => new OrientDBQueryProvider();

        public IEnumerator<OrientDBEntity> GetEnumerator()
        {
            return (Provider.Execute<IEnumerable<OrientDBEntity>>(Expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (Provider.Execute<IEnumerable>(Expression)).GetEnumerator();
        }
    }
}
