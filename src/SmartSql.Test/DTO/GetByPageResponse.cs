using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Test.DTO
{
    public class GetByPageResponse<TItem>
    {
        public IEnumerable<TItem> List { get; set; }
        public int Total { get; set; }
        public String UserName { get; set; }
    }
}