using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Annotations;

namespace SmartSql.Test.Entities
{
    public class UserExtendedInfo
    {
        public virtual long UserId { get; set; }

        [Column(FieldType = typeof(String), TypeHandler = "Json")]
        public virtual UserInfo Data { get; set; }
    }
}