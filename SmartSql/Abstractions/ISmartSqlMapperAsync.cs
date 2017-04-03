using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Abstractions
{
    public interface ISmartSqlMapperAsync
    {
        Task<int> ExecuteAsync(IRequestContext context, IDbConnectionSession session);
        Task<T> ExecuteScalarAsync<T>(IRequestContext context, IDbConnectionSession session);
        Task<IEnumerable<T>> QueryAsync<T>(IRequestContext context, IDbConnectionSession session);
        Task<T> QuerySingleAsync<T>(IRequestContext context, IDbConnectionSession session);
    }
}
