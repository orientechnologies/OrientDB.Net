using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OrientDB.Net.SqlCommandBuilder.Queryable
{
    public class OrientDBQueryProvider : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            Type elementType = expression.Type;
            try
            {
                return (IQueryable<TElement>)Activator.CreateInstance(typeof(QueryableOrientDBData<>).MakeGenericType(elementType),
                    new object[] { this, expression });
            }
            catch(TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
