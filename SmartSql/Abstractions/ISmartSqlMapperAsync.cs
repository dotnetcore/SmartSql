using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Abstractions
{
    public interface ISmartSqlMapperAsync
    {
        Task<int> ExecuteAsync(IRequestContext context);
        Task<T> ExecuteScalarAsync<T>(IRequestContext context);
        Task<IEnumerable<T>> QueryAsync<T>(IRequestContext context);
        Task<IEnumerable<T>> QueryAsync<T>(IRequestContext context, DataSourceChoice sourceChoice);
        Task<T> QuerySingleAsync<T>(IRequestContext context);
        Task<T> QuerySingleAsync<T>(IRequestContext context, DataSourceChoice sourceChoice);
    }
}
