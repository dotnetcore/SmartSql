using System;
using System.Collections.Generic;
using SmartSql.DbSession;

namespace SmartSql
{
    public static partial class DbSessionExtensions
    {
        public static IList<dynamic> QueryDynamic(this IDbSession dbSession, AbstractRequestContext requestContext)
        {
            return dbSession.Query<dynamic>(requestContext);
        }

        public static IList<IDictionary<String, object>> QueryDictionary(this IDbSession dbSession,
            AbstractRequestContext requestContext)
        {
            return dbSession.Query<IDictionary<String, object>>(requestContext);
        }

        public static dynamic QuerySingleDynamic(this IDbSession dbSession, AbstractRequestContext requestContext)
        {
            return dbSession.QuerySingle<dynamic>(requestContext);
        }

        public static  IDictionary<String, object> QuerySingleDictionary(this IDbSession dbSession,
            AbstractRequestContext requestContext)
        {
            return  dbSession.QuerySingle<IDictionary<string, object>>(requestContext);
        }
    }
}