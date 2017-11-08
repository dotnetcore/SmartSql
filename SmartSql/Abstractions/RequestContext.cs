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
        internal DynamicParameters RequestParameters { get; set; }
        private object requestObj;
        public Object Request
        {
            get { return requestObj; }
            set
            {
                requestObj = value;
                if (requestObj == null) { return; }
                RequestParameters = new DynamicParameters(requestObj);

                //if ((requestObj is DynamicParameters) || (requestObj is IEnumerable<KeyValuePair<string, object>>))
                //{
                //    RequestParameters.AddDynamicParams(requestObj);
                //}
                //else
                //{
                //    var properties = requestObj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                //    foreach (var property in properties)
                //    {
                //        var propertyVal = property.GetValue(requestObj);
                //        RequestParameters.Add(property.Name, propertyVal);
                //    }
                //}
            }
        }
    }
}
