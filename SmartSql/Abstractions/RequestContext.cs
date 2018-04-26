using Dapper;
using SmartSql.Abstractions.DataSource;
using SmartSql.Configuration;
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
        public IDataSource DataSource { get; internal set; }
        public String Scope { get; set; }
        public String SqlId { get; set; }
        public String FullSqlId => $"{Scope}.{SqlId}";


        internal SmartSqlMap SmartSqlMap { get; set; }
        internal DynamicParameters DapperParameters { get; set; }
        internal IDictionary<string, Object> RequestParameters { get; set; }
        private object _requestObj;
        public Object Request
        {
            get { return _requestObj; }
            set
            {
                _requestObj = value;
                if (_requestObj == null)
                {
                    DapperParameters = null;
                    RequestParameters = null;
                    return;
                }
                DapperParameters = new DynamicParameters(_requestObj);

                RequestParameters = new Dictionary<string, Object>();
                if (_requestObj is IEnumerable<KeyValuePair<string, object>> reqDic)
                {
                    foreach (var kv in reqDic)
                    {
                        RequestParameters.Add(kv.Key, kv.Value);
                    }
                    return;
                }
                var properties = _requestObj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in properties)
                {
                    var propertyVal = property.GetValue(_requestObj);
                    RequestParameters.Add(property.Name, propertyVal);
                }
            }
        }
    }
}
