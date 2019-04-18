using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Annotations;

namespace SmartSql.Test.Entities
{
    public class UserExtendedInfo
    {
        public long UserId { get; set; }
        [Column(FieldType = typeof(String))]
        public UserInfo Data { get; set; }
    }
}
