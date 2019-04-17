using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Exceptions;

namespace SmartSql.Configuration.Tags
{
    public class TagMinMatchedFailException : SmartSqlException
    {
        public TagMinMatchedFailException(Dynamic tag, int matched)
            : base($"Statement:{tag.Statement.FullSqlId} Tag:{tag.GetType().Name} Min-matched:{tag.Min} but matched:{matched} fail.")
        {

        }
    }
}
