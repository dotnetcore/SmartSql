using Dapper;
using SmartSql.SqlMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SmartSql.Abstractions
{
    /// <summary>
    /// Sql 请求上下文
    /// </summary>
    public class RequestContext
    {
        public String Scope { get; set; }
        public String SqlId { get; set; }
        public String FullSqlId => $"{Scope}.{SqlId}";
        internal SmartSqlMap SmartSqlMap { get; set; }
        internal DynamicParameters DapperParameters { get; set; }
        internal IDictionary<string, Object> RequestParameters { get; set; }
        private object requestObj;
        public Object Request
        {
            get { return requestObj; }
            set
            {
                requestObj = value;
                if (requestObj == null)
                {
                    DapperParameters = null;
                    RequestParameters = null;
                    return;
                }
                DapperParameters = new DynamicParameters(requestObj);
                RequestParameters = new Dictionary<string, Object>();
                if (requestObj is IEnumerable<KeyValuePair<string, object>> reqDic)
                {
                    foreach (var kv in reqDic)
                    {
                        RequestParameters.Add(kv.Key, kv.Value);
                    }
                    return;
                }
                var properties = requestObj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in properties)
                {
                    var propertyVal = property.GetValue(requestObj);
                    RequestParameters.Add(property.Name, propertyVal);
                }
            }
        }
    }
}
