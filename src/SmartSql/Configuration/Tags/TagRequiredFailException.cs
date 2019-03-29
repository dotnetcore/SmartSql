using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class TagRequiredFailException : SmartSqlException
    {
        public TagRequiredFailException(Tag tag)
            : base($"Statement:{tag.Statement.FullSqlId} Tag:{tag.GetType().Name} Required fail.")
        {

        }
    }
}
