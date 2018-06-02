using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.DataAccess.Abstractions
{
    public interface IEntity<TPrimary>
    {
         TPrimary Id { get; set; }
    }
}
