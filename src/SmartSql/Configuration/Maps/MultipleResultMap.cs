using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Maps
{
    public class MultipleResultMap
    {
        public string Id { get; set; }
        public Result Root { get; set; }
        public List<Result> Results { get; set; }
    }
    public class Result
    {
        public int Index { get; set; }
        public string Property { get; set; }
        public string MapId { get; set; }
        public ResultMap Map { get; set; }
    }

}
