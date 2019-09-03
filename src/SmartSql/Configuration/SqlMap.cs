using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using SmartSql.AutoConverter;

namespace SmartSql.Configuration
{
    public class SqlMap
    {
        public SmartSqlConfig SmartSqlConfig { get; set; }
        public String Path { get; set; }
        public String Scope { get; set; }
        public IDictionary<String, Cache> Caches { get; set; }
        public IDictionary<String, Statement> Statements { get; set; }
        public IDictionary<String, ParameterMap> ParameterMaps { get; set; }
        public IDictionary<String, ResultMap> ResultMaps { get; set; }
        public IDictionary<String, MultipleResultMap> MultipleResultMaps { get; set; }

        public IAutoConverter AutoConverter { get; set; }
        
        public Statement GetStatement(string fullSqlId)
        {
            if (!Statements.TryGetValue(fullSqlId, out var statement))
            {
                throw new SmartSqlException($"Can not find Statement.FullSqlId:{fullSqlId}");
            }
            return statement;
        }
        public Cache GetCache(string cacheId)
        {
            if (!Caches.TryGetValue(cacheId, out var cache))
            {
                throw new SmartSqlException($"Can not find Cache.Id:{cacheId}");
            }
            return cache;
        }
        public ParameterMap GetParameterMap(string parameterMapId)
        {
            if (!ParameterMaps.TryGetValue(parameterMapId, out var parameterMap))
            {
                throw new SmartSqlException($"Can not find ParameterMap.Id:{parameterMapId}");
            }
            return parameterMap;
        }
        public ResultMap GetResultMap(string resultMapId)
        {
            if (!ResultMaps.TryGetValue(resultMapId, out var resultMap))
            {
                throw new SmartSqlException($"Can not find ResultMap.Id:{resultMapId}");
            }
            return resultMap;
        }

        public MultipleResultMap GetMultipleResultMap(string multipleResultMapId)
        {
            if (!MultipleResultMaps.TryGetValue(multipleResultMapId, out var multipleResultMap))
            {
                throw new SmartSqlException($"Can not find MultipleResultMap.Id:{multipleResultMapId}");
            }
            return multipleResultMap;
        }
    }
}
