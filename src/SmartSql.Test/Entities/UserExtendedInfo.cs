using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Annotations;

namespace SmartSql.Test.Entities
{
    [Table("T_UserExtendedInfo")]
    public class UserExtendedInfo
    {
        public virtual long UserId { get; set; }

        [Column(FieldType = typeof(String),Alias = "GlobalSmartSql",TypeHandler = "Json")]
        public virtual UserInfo Data { get; set; }
    }
}