using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Abstractions
{
    public interface ISmartSqlMapperAsync
    {
        Task<int> ExecuteAsync(RequestContext context);
        Task<T> ExecuteScalarAsync<T>(RequestContext context);
        Task<IEnumerable<T>> QueryAsync<T>(RequestContext context);
        Task<T> QuerySingleAsync<T>(RequestContext context);
        Task<IMultipleResult> FillMultipleAsync(RequestContext context, IMultipleResult multipleResult);
        Task<DataTable> GetDataTableAsync(RequestContext context);
        Task<DataSet> GetDataSetAsync(RequestContext context);
        Task<T> GetNestedAsync<T>(RequestContext context);
    }
}
