using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration
{
    public class MultipleResultMap
    {
        public string Id { get; set; }
        public Result Root { get; set; }
        public List<Result> Results { get; set; }

        private Result GetResult(int resultIndex)
        {
            return Results[resultIndex];
        }

        public ResultMap GetResultMap(int resultIndex)
        {
            return GetResult(resultIndex)?.Map;
        }
    }

    public class Result
    {
        public const String ROOT_PROPERTY = "__ROOT__";
        public string Property { get; set; }
        public string MapId { get; set; }
        public ResultMap Map { get; set; }
    }
}