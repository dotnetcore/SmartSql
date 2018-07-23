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
        public IDictionary<string, object> RequestParameters { get; internal set; }
        public object Request { get; set; }

        [Obsolete("Internal call")]
        public void Setup(SmartSqlContext smartSqlContext)
        {
            SmartSqlContext = smartSqlContext;
            SetupParameters();
        }
        internal void SetupParameters()
        {
            if (CommandType == CommandType.StoredProcedure || Request == null)
            {
                return;
            }

            bool ignoreParameterCase = SmartSqlContext.IgnoreParameterCase;
            var paramComparer = ignoreParameterCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;
            if (Request is Dictionary<string, object> reqDicParams)
            {
                if (reqDicParams.Comparer == paramComparer)
                {
                    RequestParameters = reqDicParams;
                }
                else
                {
                    RequestParameters = new Dictionary<string, object>(reqDicParams, paramComparer);
                }
                return;
            }
            if (Request is IEnumerable<KeyValuePair<string, object>> reqDic)
            {
                RequestParameters = new Dictionary<string, object>(paramComparer);
                foreach (var kv in reqDic)
                {
                    RequestParameters.Add(kv.Key, kv.Value);
                }
                return;
            }
            RequestParameters = ObjectUtils.ToDictionary(Request, ignoreParameterCase);
        }

        public String StatementKey { get { return (!String.IsNullOrEmpty(FullSqlId) ? FullSqlId : RealSql); } }

        public String Key { get { return $"{StatementKey}:{RequestString}"; } }

        public String RequestString
        {
            get
            {
                if (RequestParameters == null) { return "Null"; }
                StringBuilder strBuilder = new StringBuilder();
                var reqParams = RequestParameters;
                foreach (var reqParam in reqParams)
                {
                    BuildSqlQueryString(strBuilder, reqParam.Key, reqParam.Value);
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
