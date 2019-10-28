using SmartSql.IdGenerator;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using SmartSql.Reflection.PropertyAccessor;

namespace SmartSql.Configuration.Tags
{
    public class IdGenerator : ITag
    {
        public IIdGenerator IdGen { get; set; }
        public string Id { get; set; }
        public Statement Statement { get; set; }
        public ITag Parent { get; set; }
        public bool Assign { get; set; }

        public void BuildSql(AbstractRequestContext context)
        {
            var nextId = IdGen.NextId();
            if (context.Parameters.TryAdd(Id, nextId)) return;
            context.Parameters.TryGetValue(Id, out Data.SqlParameter sqlParameter);
            sqlParameter.Value = nextId;
            if (!Assign) return;
            var sourceRequest = context.GetRequest();
            EmitSetAccessorFactory.Instance.Create(sourceRequest.GetType(), Id)(sourceRequest, nextId);
        }

        public bool IsCondition(AbstractRequestContext context)
        {
            return true;
        }
    }
}