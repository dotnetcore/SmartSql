using SmartSql.Abstractions.TypeHandler;
using SmartSql.Configuration.Statements;
using SmartSql.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;

namespace SmartSql.Abstractions
{
    /// <summary>
    /// Sql 请求上下文
    /// </summary>
    public class RequestContext
    {
        //public Guid Id { get; } = Guid.NewGuid();
        public SmartSqlContext SmartSqlContext { get; internal set; }
        public DataSourceChoice DataSourceChoice { get; set; } = DataSourceChoice.Unknow;
        public CommandType CommandType { get; set; } = CommandType.Text;
        public Statement Statement { get; internal set; }
        public StringBuilder Sql { get; internal set; }
        public bool IsStatementSql { get; internal set; } = true;
        internal bool IgnorePrepend { get; set; } = false;
        public String RealSql { get; set; }
        public String Scope { get; set; }
        public String SqlId { get; set; }
        public String FullSqlId => $"{Scope}.{SqlId}";
        public DbParameterCollection RequestParameters { get; internal set; }
        public object Request { get; set; }

        [Obsolete("Internal call")]
        public void Setup(SmartSqlContext smartSqlContext)
        {
            SmartSqlContext = smartSqlContext;
            SetupParameters();
        }
        internal void SetupParameters()
        {
            bool ignoreParameterCase = SmartSqlContext.IgnoreParameterCase;
            if (Request is DbParameterCollection dbParameterCollection
                && dbParameterCollection.IgnoreParameterCase == ignoreParameterCase
                )
            {
                RequestParameters = dbParameterCollection;
            }
            else
            {
                RequestParameters = new DbParameterCollection(ignoreParameterCase, Request);
            }
        }

        public String StatementKey { get { return (!String.IsNullOrEmpty(SqlId) ? FullSqlId : RealSql); } }

        public String Key { get { return $"{StatementKey}:{RequestString}"; } }

        public String RequestString
        {
            get
            {
                if (RequestParameters == null) { return "Null"; }
                StringBuilder strBuilder = new StringBuilder();
                var reqParams = RequestParameters;
                foreach (var reqParamName in reqParams.ParameterNames)
                {
                    var reqParamVal = reqParams.GetValue(reqParamName);
                    BuildSqlQueryString(strBuilder, reqParamName, reqParamVal);
                }
                return strBuilder.ToString().Trim('&');
            }
        }

        private void BuildSqlQueryString(StringBuilder strBuilder, string key, object val)
        {
            if (val is IEnumerable list && !(val is String))
            {
                strBuilder.AppendFormat("&{0}=(", key);
                foreach (var item in list)
                {
                    strBuilder.AppendFormat("{0},", item);
                }
                strBuilder.Append(")");
            }
            else
            {
                strBuilder.AppendFormat("&{0}={1}", key, val);
            }
        }

        public ITypeHandler GetTypeHandler(string typeHandlerName)
        {
            return SmartSqlContext.SqlMapConfig.TypeHandlers.FirstOrDefault(th => th.Name == typeHandlerName)?.Handler;
        }

    }
}
