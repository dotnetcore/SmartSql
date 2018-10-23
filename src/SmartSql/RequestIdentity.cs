using SmartSql.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartSql
{
    public class RequestIdentity
    {
        private readonly RequestContext _context;
        public String StatementKey { get; private set; }
        public string GetKey()
        {
            if (String.IsNullOrEmpty(_key))
            {
                InitKey();
            }
            return _key;
        }
        private string _key;

        public RequestIdentity(RequestContext context)
        {
            _context = context;
            InitStatementKey();
        }

        private void InitStatementKey()
        {
            StatementKey = !String.IsNullOrEmpty(_context.SqlId) ? _context.FullSqlId : _context.RealSql;
        }

        private void InitKey()
        {
            var requestStr = String.Empty;
            if (_context.RequestParameters == null)
            {
                requestStr = "Null";
            }
            else
            {
                StringBuilder strBuilder = new StringBuilder();
                var paramNames = _context.SmartSqlContext.SqlParamAnalyzer.Analyse(_context.RealSql).OrderBy(m => m);
                foreach (var paramName in paramNames)
                {
                    var paramMap = _context.ParameterMap?.Parameters?.FirstOrDefault(p => p.Name == paramName);
                    var propertyName = paramMap != null ? paramMap.Property : paramName;
                    if (!_context.RequestParameters.Contains(propertyName)) { continue; }
                    var dbParameter = _context.RequestParameters.Get(propertyName);
                    var typeHandler = dbParameter.TypeHandler ?? paramMap?.Handler;
                    var reqVal = dbParameter.Value;
                    if (typeHandler != null)
                    {
                        reqVal = typeHandler.GetSetParameterValue(reqVal);
                    }
                    BuildSqlQueryString(strBuilder, paramName, reqVal);
                }
                requestStr = strBuilder.ToString().Trim('&');
            }
            _key = $"{StatementKey}:{requestStr}";
        }
        private void BuildSqlQueryString(StringBuilder strBuilder, string key, object val)
        {
            if (val == null)
            {
                strBuilder.AppendFormat("&{0}=Null", key);
            }
            else
            {
                strBuilder.AppendFormat("&{0}={1}", key, val);
            }
        }
    }
}
