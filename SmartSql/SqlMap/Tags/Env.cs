using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.SqlMap.Tags
{
    public class Env : Tag
    {
        public override TagType Type => TagType.Env;
        public string DbProvider { get; set; }
        public override bool IsCondition(RequestContext context)
        {
            var dataBase = context.SmartSqlMap.SmartSqlMapConfig.Database;
            if (dataBase.DbProvider.Name == DbProvider)
            {
                return true;
            }
            return false;
        }
    }
}
